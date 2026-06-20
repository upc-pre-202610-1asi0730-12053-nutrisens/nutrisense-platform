using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;

public interface INutritionLogQueryService
{
    Task<IEnumerable<NutritionLog>> Handle(GetNutritionLogByUserAndDateQuery query, CancellationToken ct = default);
    Task<DailyMacroSummary> Handle(GetDailyMacroSummaryQuery query, CancellationToken ct = default);
    Task<IEnumerable<NutritionLog>> Handle(GetNutritionLogsByUserQuery query, CancellationToken ct = default);
}
