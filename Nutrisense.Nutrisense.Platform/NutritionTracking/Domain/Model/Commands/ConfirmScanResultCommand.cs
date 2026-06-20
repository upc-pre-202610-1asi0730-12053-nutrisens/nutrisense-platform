namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

public record ConfirmScanResultCommand(
    int UserId,
    int DetectedFoodId,
    decimal QuantityG,
    string MealType,
    DateOnly Date);
