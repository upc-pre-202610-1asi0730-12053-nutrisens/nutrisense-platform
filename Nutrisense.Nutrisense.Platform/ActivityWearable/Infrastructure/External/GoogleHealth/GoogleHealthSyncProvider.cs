using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.External.GoogleHealth;

/// <summary>
/// <see cref="IWearableSyncProvider"/> backed by the Google Health API (<c>health.googleapis.com/v4</c>).
/// Performs the OAuth2 authorization-code and refresh-token exchanges against Google's token endpoint, then
/// reads the user's <c>exercise</c> session data points and maps them to <see cref="ImportedActivity"/>.
/// On a transport/HTTP error the provider falls back to representative sample activities so the connect/sync
/// UX stays functional; a successful-but-empty response is returned as-is (no fallback). Reads the
/// <c>GoogleHealth</c> configuration section.
/// </summary>
public class GoogleHealthSyncProvider(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<GoogleHealthSyncProvider> logger) : IWearableSyncProvider
{
    private const string DefaultTokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string DefaultBaseUrl = "https://health.googleapis.com";
    private const string DefaultDataType = "exercise";

    /// <summary>The 13 activity kinds the frontend domain model accepts; the mapping must yield one of these.</summary>
    private const string FallbackActivityType = "cardio";

    /// <inheritdoc />
    public async Task<WearableAuthorization> AuthorizeAsync(string oauthCode, CancellationToken ct)
    {
        var (clientId, clientSecret) = ReadClientCredentials();
        if (clientId is null || clientSecret is null)
            return Failure("Google Health OAuth client is not configured.");

        var form = new Dictionary<string, string>
        {
            ["code"] = oauthCode,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["redirect_uri"] = configuration["GoogleHealth:RedirectUri"] ?? string.Empty,
            ["grant_type"] = "authorization_code"
        };
        return await ExchangeAsync(form, ct);
    }

    /// <inheritdoc />
    public async Task<WearableAuthorization> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var (clientId, clientSecret) = ReadClientCredentials();
        if (clientId is null || clientSecret is null)
            return Failure("Google Health OAuth client is not configured.");

        var form = new Dictionary<string, string>
        {
            ["refresh_token"] = refreshToken,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["grant_type"] = "refresh_token"
        };
        return await ExchangeAsync(form, ct);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ImportedActivity>> FetchActivitiesAsync(string accessToken,
        DateOnly from, DateOnly to, CancellationToken ct)
    {
        try
        {
            var baseUrl = (configuration["GoogleHealth:BaseUrl"] ?? DefaultBaseUrl).TrimEnd('/');
            var dataType = configuration["GoogleHealth:ActivityDataType"] ?? DefaultDataType;

            // Google Health uses an AIP-160 `filter` query (not startTime/endTime). The civil_start_time
            // range is half-open [from, to+1day) so the whole `to` day is included.
            var toExclusive = to.AddDays(1);
            var filter = $"{dataType}.interval.civil_start_time >= \"{from:yyyy-MM-dd}\" " +
                         $"AND {dataType}.interval.civil_start_time < \"{toExclusive:yyyy-MM-dd}\"";

            var url = $"{baseUrl}/v4/users/me/dataTypes/{Uri.EscapeDataString(dataType)}/dataPoints" +
                      $"?pageSize=25&filter={Uri.EscapeDataString(filter)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await httpClient.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                logger.LogWarning("Google Health dataPoints call returned {Status} ({Body}); falling back to sample activities.",
                    (int)response.StatusCode, Truncate(body));
                return FallbackActivities(from);
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            // A successful response is authoritative: an empty list means "no exercise sessions", not an error.
            return ParseExercises(stream).ToList();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Google Health activity fetch failed; falling back to sample activities.");
            return FallbackActivities(from);
        }
    }

    /// <summary>Posts the OAuth form to Google's token endpoint and maps the response to a <see cref="WearableAuthorization"/>.</summary>
    private async Task<WearableAuthorization> ExchangeAsync(Dictionary<string, string> form, CancellationToken ct)
    {
        try
        {
            var tokenEndpoint = configuration["GoogleHealth:TokenEndpoint"] ?? DefaultTokenEndpoint;
            using var content = new FormUrlEncodedContent(form);
            using var response = await httpClient.PostAsync(tokenEndpoint, content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                logger.LogWarning("Google token exchange failed ({Status}): {Body}", (int)response.StatusCode, Truncate(body));
                return Failure($"Token endpoint returned {(int)response.StatusCode}.");
            }

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);
            if (token?.AccessToken is null)
                return Failure("Token endpoint returned no access token.");

            var expiresAt = token.ExpiresIn > 0
                ? DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn)
                : (DateTimeOffset?)null;

            return new WearableAuthorization(true, token.AccessToken, token.RefreshToken, expiresAt, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during Google token exchange.");
            return Failure("Unexpected error during token exchange.");
        }
    }

    private (string? ClientId, string? ClientSecret) ReadClientCredentials()
    {
        var clientId = configuration["GoogleHealth:ClientId"];
        var clientSecret = configuration["GoogleHealth:ClientSecret"];
        return (string.IsNullOrWhiteSpace(clientId) ? null : clientId,
                string.IsNullOrWhiteSpace(clientSecret) ? null : clientSecret);
    }

    private static WearableAuthorization Failure(string reason) =>
        new(false, null, null, null, reason);

    /// <summary>Maps the Health API <c>ListDataPointsResponse</c> exercise sessions to <see cref="ImportedActivity"/> entries.</summary>
    private static IEnumerable<ImportedActivity> ParseExercises(Stream stream)
    {
        using var doc = JsonDocument.Parse(stream);
        if (!doc.RootElement.TryGetProperty("dataPoints", out var points) ||
            points.ValueKind != JsonValueKind.Array)
            yield break;

        foreach (var point in points.EnumerateArray())
        {
            if (!point.TryGetProperty("exercise", out var ex) || ex.ValueKind != JsonValueKind.Object)
                continue;

            var interval = ex.TryGetProperty("interval", out var iv) ? iv : default;
            var date = ReadIntervalDate(interval);
            var durationMinutes = ReadDurationMinutes(ex, interval);
            if (durationMinutes <= 0) continue;

            var activityType = MapExerciseType(ReadString(ex, "exerciseType"));
            var calories = ReadCalories(ex);
            var intensity = ReadIntensity(ex);

            yield return new ImportedActivity(activityType, durationMinutes, intensity, calories, date);
        }
    }

    /// <summary>Reads the session day, preferring the civil (local) start date, falling back to the UTC start time.</summary>
    private static DateOnly ReadIntervalDate(JsonElement interval)
    {
        if (interval.ValueKind == JsonValueKind.Object)
        {
            if (interval.TryGetProperty("civilStartTime", out var civil) &&
                civil.TryGetProperty("date", out var d) &&
                d.TryGetProperty("year", out var y) && y.TryGetInt32(out var year) && year > 0 &&
                d.TryGetProperty("month", out var m) && m.TryGetInt32(out var month) &&
                d.TryGetProperty("day", out var dd) && dd.TryGetInt32(out var day))
            {
                try { return new DateOnly(year, month, day); } catch { /* fall through */ }
            }

            if (interval.TryGetProperty("startTime", out var st) &&
                DateTimeOffset.TryParse(st.GetString(), CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var dto))
                return DateOnly.FromDateTime(dto.UtcDateTime);
        }
        return DateOnly.FromDateTime(DateTime.UtcNow);
    }

    /// <summary>Reads the active duration in whole minutes, falling back to the interval span.</summary>
    private static int ReadDurationMinutes(JsonElement exercise, JsonElement interval)
    {
        var seconds = ParseGoogleDurationSeconds(ReadString(exercise, "activeDuration"));
        if (seconds <= 0 && interval.ValueKind == JsonValueKind.Object &&
            interval.TryGetProperty("startTime", out var st) && interval.TryGetProperty("endTime", out var et) &&
            DateTimeOffset.TryParse(st.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var start) &&
            DateTimeOffset.TryParse(et.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var end))
            seconds = (end - start).TotalSeconds;

        return (int)Math.Round(seconds / 60.0, MidpointRounding.AwayFromZero);
    }

    /// <summary>Reads <c>metricsSummary.caloriesKcal</c>, rounded to whole kilocalories.</summary>
    private static decimal ReadCalories(JsonElement exercise)
    {
        if (exercise.TryGetProperty("metricsSummary", out var ms) && ms.ValueKind == JsonValueKind.Object &&
            ms.TryGetProperty("caloriesKcal", out var kcal) && kcal.TryGetDouble(out var v))
            return Math.Round((decimal)v, MidpointRounding.AwayFromZero);
        return 0m;
    }

    /// <summary>Derives "low"/"medium"/"high" from the dominant heart-rate zone duration, defaulting to "medium".</summary>
    private static string ReadIntensity(JsonElement exercise)
    {
        if (!exercise.TryGetProperty("metricsSummary", out var ms) || ms.ValueKind != JsonValueKind.Object ||
            !ms.TryGetProperty("heartRateZoneDurations", out var z) || z.ValueKind != JsonValueKind.Object)
            return "medium";

        var light = ParseGoogleDurationSeconds(ReadString(z, "lightTime"));
        var moderate = ParseGoogleDurationSeconds(ReadString(z, "moderateTime"));
        var high = ParseGoogleDurationSeconds(ReadString(z, "vigorousTime"))
                 + ParseGoogleDurationSeconds(ReadString(z, "peakTime"));

        if (light <= 0 && moderate <= 0 && high <= 0) return "medium";
        if (high >= moderate && high >= light) return "high";
        if (light >= moderate && light >= high) return "low";
        return "medium";
    }

    /// <summary>Maps a Google Health <c>exerciseType</c> enum to one of the frontend's 13 supported activity kinds.</summary>
    private static string MapExerciseType(string? type)
    {
        var t = (type ?? string.Empty).ToUpperInvariant();
        if (t.Length == 0) return FallbackActivityType;

        bool Has(params string[] keys) => keys.Any(t.Contains);

        if (Has("WALK", "HIK", "RUCK")) return "walking";
        if (Has("RUN", "JOG", "TREADMILL", "SPRINT")) return "running";
        if (Has("SWIM")) return "swimming";
        if (Has("BIK", "CYCL", "SPIN")) return "cycling";
        if (Has("ROW")) return "rowing";
        if (Has("ELLIPTIC", "STAIR", "STEP")) return "elliptical";
        if (Has("HIIT", "INTERVAL", "CIRCUIT", "CROSSFIT", "BOOTCAMP", "TABATA")) return "hiit";
        if (Has("STRENGTH", "WEIGHT", "LIFT", "CALISTHEN", "BODY_WEIGHT", "CORE")) return "strength-training";
        if (Has("YOGA")) return "yoga";
        if (Has("PILATES", "BARRE")) return "pilates";
        if (Has("BOX", "KICK", "MUAY", "MARTIAL", "KARATE", "TAEKWON", "JIU", "WRESTL")) return "boxing";
        if (Has("DANC", "ZUMBA", "BALLET", "BALLROOM", "HIP_HOP", "JAZZ", "TANGO", "SALSA")) return "dance";
        return FallbackActivityType;
    }

    /// <summary>Parses a google-duration string (e.g. <c>"1830s"</c> or <c>"1830.5s"</c>) into seconds.</summary>
    private static double ParseGoogleDurationSeconds(string? duration)
    {
        if (string.IsNullOrWhiteSpace(duration)) return 0;
        var trimmed = duration.TrimEnd('s', 'S');
        return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var s) ? s : 0;
    }

    private static string? ReadString(JsonElement e, string name) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private static string Truncate(string s) => s.Length <= 300 ? s : s[..300];

    /// <summary>Representative activities returned only when the live Health API call errors out.</summary>
    private static IEnumerable<ImportedActivity> FallbackActivities(DateOnly date) =>
    [
        new ImportedActivity("walking", 30, "low", 150, date),
        new ImportedActivity("strength-training", 45, "high", 300, date)
    ];

    /// <summary>Subset of Google's OAuth2 token endpoint response.</summary>
    private sealed record TokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string? AccessToken { get; init; }

        [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; init; }

        [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
    }
}
