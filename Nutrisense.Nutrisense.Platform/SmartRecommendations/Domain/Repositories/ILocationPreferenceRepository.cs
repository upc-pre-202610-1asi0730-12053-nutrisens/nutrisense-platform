using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

public interface ILocationPreferenceRepository : IBaseRepository<LocationPreference>
{
    Task<LocationPreference?> FindByUserIdAsync(int userId, CancellationToken ct = default);
    Task<LocationPreference> FindOrCreateAsync(int userId, CancellationToken ct = default);
}
