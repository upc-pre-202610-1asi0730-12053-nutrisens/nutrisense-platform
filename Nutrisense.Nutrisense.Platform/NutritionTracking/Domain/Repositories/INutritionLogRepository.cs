using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;

public interface INutritionLogRepository : IBaseRepository<NutritionLog>
{
    Task<IEnumerable<NutritionLog>> FindByUserAndDateAsync(int userId, DateOnly date, CancellationToken ct = default);
    Task<IEnumerable<NutritionLog>> FindByUserAsync(int userId, DateOnly? from, DateOnly? to, CancellationToken ct = default);
    Task<DailyMacroSummary> GetDailyMacroSummaryAsync(int userId, DateOnly date, CancellationToken ct = default);
}
