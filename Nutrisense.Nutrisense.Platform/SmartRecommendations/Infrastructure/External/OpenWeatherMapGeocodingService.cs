using System.Globalization;
using System.Net.Http.Json;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External;

/// <summary>
/// Geocoding via the OpenWeatherMap Geocoding API (<c>/geo/1.0</c>). Forward search is by city
/// name only (no country coercion — disambiguation is left to the caller/UI), reverse maps
/// coordinates to the nearest known place.
/// </summary>
public class OpenWeatherMapGeocodingService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<OpenWeatherMapGeocodingService> logger) : IGeocodingService
{
    public async Task<IReadOnlyList<GeoCityCandidate>> SearchAsync(string query, int limit, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];

        var apiKey = configuration["OpenWeatherMap:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("OpenWeatherMap API key not configured; geocoding search disabled.");
            return [];
        }

        try
        {
            var url = $"{GeoBaseUrl()}/direct?q={Uri.EscapeDataString(query.Trim())}&limit={Math.Clamp(limit, 1, 5)}&appid={apiKey}";
            var entries = await httpClient.GetFromJsonAsync<List<OpenWeatherMapGeoEntry>>(url, ct);
            return entries?.Select(ToCandidate).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error geocoding query '{Query}' via OpenWeatherMap.", query);
            return [];
        }
    }

    public async Task<GeoCityCandidate?> ReverseAsync(decimal lat, decimal lng, CancellationToken ct = default)
    {
        var apiKey = configuration["OpenWeatherMap:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("OpenWeatherMap API key not configured; reverse geocoding disabled.");
            return null;
        }

        try
        {
            var latS = lat.ToString(CultureInfo.InvariantCulture);
            var lngS = lng.ToString(CultureInfo.InvariantCulture);
            var url = $"{GeoBaseUrl()}/reverse?lat={latS}&lon={lngS}&limit=1&appid={apiKey}";
            var entries = await httpClient.GetFromJsonAsync<List<OpenWeatherMapGeoEntry>>(url, ct);
            var first = entries?.FirstOrDefault();
            return first is null ? null : ToCandidate(first);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reverse-geocoding ({Lat},{Lng}) via OpenWeatherMap.", lat, lng);
            return null;
        }
    }

    private string GeoBaseUrl() =>
        configuration["OpenWeatherMap:GeoBaseUrl"]?.TrimEnd('/') ?? "https://api.openweathermap.org/geo/1.0";

    private static GeoCityCandidate ToCandidate(OpenWeatherMapGeoEntry e)
    {
        var en = e.LocalNames is not null && e.LocalNames.TryGetValue("en", out var enName) ? enName : e.Name;
        var es = e.LocalNames is not null && e.LocalNames.TryGetValue("es", out var esName) ? esName : e.Name;
        return new GeoCityCandidate(e.Name, en, es, e.Country, e.State, e.Lat, e.Lon);
    }
}
