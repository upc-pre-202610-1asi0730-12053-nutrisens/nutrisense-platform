namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;

public record GetDailyMacroSummaryQuery(int UserId, DateOnly Date);
