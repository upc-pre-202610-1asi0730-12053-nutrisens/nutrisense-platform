namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

public record LogMealToDailyLogCommand(
    int UserId,
    int FoodId,
    string MealType,
    DateOnly Date,
    decimal QuantityG,
    string Source);
