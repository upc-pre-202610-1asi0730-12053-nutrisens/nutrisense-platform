namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

public record SelectMenuOptionCommand(
    int UserId,
    int FoodId,
    string MealType,
    DateOnly Date,
    decimal QuantityG);
