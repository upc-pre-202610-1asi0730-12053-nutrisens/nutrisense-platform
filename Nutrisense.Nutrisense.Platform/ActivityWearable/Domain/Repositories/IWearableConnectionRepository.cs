using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;

/// <summary>Repository of <see cref="WearableConnection"/> aggregates.</summary>
public interface IWearableConnectionRepository : IBaseRepository<WearableConnection>
{
    /// <summary>Retrieves all wearable connections belonging to a user.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The user's wearable connections across all providers and states.</returns>
    Task<IEnumerable<WearableConnection>> FindByUserIdAsync(int userId, CancellationToken ct = default);

    /// <summary>Retrieves the active connection a user holds for a given provider, if any.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="provider">External provider to match (e.g. "google-fit").</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The matching connection, or null if none exists.</returns>
    Task<WearableConnection?> FindByUserAndProviderAsync(int userId, string provider, CancellationToken ct = default);
}
