using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.QueryServices;

/// <summary>Contract for handling wearable-connection read operations.</summary>
public interface IWearableConnectionQueryService
{
    /// <summary>Retrieves all wearable connections belonging to a user.</summary>
    /// <param name="query">The query carrying the user identifier.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The user's wearable connections.</returns>
    Task<IEnumerable<WearableConnection>> Handle(GetWearableConnectionsByUserIdQuery query, CancellationToken ct = default);
}
