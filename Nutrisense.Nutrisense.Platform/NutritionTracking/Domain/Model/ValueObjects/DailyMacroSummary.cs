namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;

public record DailyMacroSummary(
    DateOnly Date,
    decimal TotalCalories,
    decimal TotalProteinG,
    decimal TotalCarbsG,
    decimal TotalFatG,
    decimal TotalFiberG,
    int MealCount);
