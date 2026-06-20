using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.External.GoogleFit;

// TODO: replace with real Google Fit OAuth + REST API
/// <summary>Implementation of <see cref="IWearableSyncProvider"/> for Google Fit. Currently a stub returning canned authorization and activity data.</summary>
public class GoogleFitSyncProvider : IWearableSyncProvider
{
    /// <summary>Stub authorization that always succeeds, returning a fake access token.</summary>
    /// <param name="oauthCode">Authorization code from the provider's consent flow (ignored by the stub).</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>A successful <see cref="WearableAuthorization"/> with a generated fake token.</returns>
    public Task<WearableAuthorization> AuthorizeAsync(string oauthCode, CancellationToken ct) =>
        Task.FromResult(new WearableAuthorization(true, "fake-access-token-" + Guid.NewGuid(), null));

    /// <summary>Stub fetch that returns a fixed pair of sample activities dated at the range's lower bound.</summary>
    /// <param name="wearableConnectionId">Identifier of the connection whose data is fetched (ignored by the stub).</param>
    /// <param name="from">Inclusive lower bound of the date range; used as the date of the sample activities.</param>
    /// <param name="to">Inclusive upper bound of the date range (ignored by the stub).</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>A fixed set of sample imported activities.</returns>
    public Task<IEnumerable<ImportedActivity>> FetchActivitiesAsync(int wearableConnectionId,
        DateOnly from, DateOnly to, CancellationToken ct)
    {
        IEnumerable<ImportedActivity> activities =
        [
            new ImportedActivity("walking", 30, "low", 150, from),
            new ImportedActivity("strength-training", 45, "high", 300, from)
        ];
        return Task.FromResult(activities);
    }
}
