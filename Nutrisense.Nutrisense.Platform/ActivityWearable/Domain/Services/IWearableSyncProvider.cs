namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;

/// <summary>Outcome of an OAuth authorization (or token refresh) attempt against a wearable provider.</summary>
/// <param name="Success">Whether the authorization succeeded.</param>
/// <param name="AccessToken">Access token granted by the provider, or null on failure.</param>
/// <param name="RefreshToken">Long-lived refresh token, or null when the provider did not return one.</param>
/// <param name="ExpiresAt">Instant (UTC) at which the access token expires, or null if unknown.</param>
/// <param name="FailureReason">Human-readable reason for failure, or null on success.</param>
public record WearableAuthorization(
    bool Success,
    string? AccessToken,
    string? RefreshToken,
    DateTimeOffset? ExpiresAt,
    string? FailureReason);

/// <summary>An activity fetched from an external wearable provider, before being mapped to a domain aggregate.</summary>
/// <param name="ActivityType">Kind of activity reported by the provider.</param>
/// <param name="DurationMinutes">Duration of the activity in minutes.</param>
/// <param name="Intensity">Effort level reported by the provider.</param>
/// <param name="CaloriesBurned">Energy expended by the activity, in kilocalories.</param>
/// <param name="Date">Calendar day the activity took place.</param>
public record ImportedActivity(string ActivityType, int DurationMinutes, string Intensity,
    decimal CaloriesBurned, DateOnly Date);

/// <summary>Abstraction over an external wearable provider's authorization and activity-retrieval API.</summary>
public interface IWearableSyncProvider
{
    /// <summary>Exchanges an OAuth authorization code for provider access and refresh tokens.</summary>
    /// <param name="oauthCode">Authorization code from the provider's consent flow.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The authorization outcome, including access/refresh tokens on success.</returns>
    Task<WearableAuthorization> AuthorizeAsync(string oauthCode, CancellationToken ct);

    /// <summary>Exchanges a refresh token for a fresh access token.</summary>
    /// <param name="refreshToken">The long-lived refresh token previously granted.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The refreshed authorization outcome. The provider may not return a new refresh token.</returns>
    Task<WearableAuthorization> RefreshAsync(string refreshToken, CancellationToken ct);

    /// <summary>Fetches the activities recorded by the provider within a date range.</summary>
    /// <param name="accessToken">Valid (non-expired) access token authorizing the call.</param>
    /// <param name="from">Inclusive lower bound of the date range.</param>
    /// <param name="to">Inclusive upper bound of the date range.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The activities reported by the provider for the range.</returns>
    Task<IEnumerable<ImportedActivity>> FetchActivitiesAsync(string accessToken,
        DateOnly from, DateOnly to, CancellationToken ct);
}
