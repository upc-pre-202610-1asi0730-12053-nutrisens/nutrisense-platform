using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;

public interface IFoodRepository : IBaseRepository<Food>
{
    Task<Food?> FindByKeyAsync(string key, CancellationToken ct = default);
    Task<IEnumerable<Food>> SearchByNameAsync(string query, string lang, CancellationToken ct = default);
    Task<Food?> FindByExternalIdAsync(string externalId, CancellationToken ct = default);
}
