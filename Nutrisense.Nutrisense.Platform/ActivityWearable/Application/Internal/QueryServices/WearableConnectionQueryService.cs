using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Internal.QueryServices;

/// <summary>Implementation of <see cref="IWearableConnectionQueryService"/>. Reads a user's wearable connections from the repository.</summary>
public class WearableConnectionQueryService(IWearableConnectionRepository repository) : IWearableConnectionQueryService
{
    /// <summary>Retrieves all wearable connections belonging to the queried user.</summary>
    /// <param name="query">The query carrying the user identifier.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The user's wearable connections.</returns>
    public async Task<IEnumerable<WearableConnection>> Handle(GetWearableConnectionsByUserIdQuery query, CancellationToken ct = default) =>
        await repository.FindByUserIdAsync(query.UserId, ct);
}
