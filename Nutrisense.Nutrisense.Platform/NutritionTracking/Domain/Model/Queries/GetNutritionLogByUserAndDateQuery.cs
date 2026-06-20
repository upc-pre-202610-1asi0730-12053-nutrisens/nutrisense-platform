namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;

public record GetNutritionLogByUserAndDateQuery(int UserId, DateOnly Date);
