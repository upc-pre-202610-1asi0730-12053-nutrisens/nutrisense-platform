using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

public interface IPantryRepository : IBaseRepository<Pantry>
{
    Task<Pantry?> FindByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Pantry> FindOrCreateAsync(int userId, CancellationToken ct = default);
}
