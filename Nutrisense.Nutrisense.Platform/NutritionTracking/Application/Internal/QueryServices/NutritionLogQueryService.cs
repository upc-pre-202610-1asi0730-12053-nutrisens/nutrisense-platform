using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal.QueryServices;

public class NutritionLogQueryService(INutritionLogRepository nutritionLogRepository) : INutritionLogQueryService
{
    public async Task<IEnumerable<NutritionLog>> Handle(GetNutritionLogByUserAndDateQuery query, CancellationToken ct = default) =>
        await nutritionLogRepository.FindByUserAndDateAsync(query.UserId, query.Date, ct);

    public async Task<DailyMacroSummary> Handle(GetDailyMacroSummaryQuery query, CancellationToken ct = default) =>
        await nutritionLogRepository.GetDailyMacroSummaryAsync(query.UserId, query.Date, ct);

    public async Task<IEnumerable<NutritionLog>> Handle(GetNutritionLogsByUserQuery query, CancellationToken ct = default) =>
        await nutritionLogRepository.FindByUserAsync(query.UserId, query.FromDate, query.ToDate, ct);
}
