namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;

public record GetNutritionLogsByUserQuery(int UserId, DateOnly? FromDate, DateOnly? ToDate);
