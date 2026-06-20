namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record ConfirmScanResource(
    int UserId,
    int DetectedFoodId,
    decimal QuantityG,
    string MealType,
    string Date)
{
    /// <summary>User ID of the person confirming the scan.</summary>
    public int UserId { get; } = UserId;

    /// <summary>Food ID detected from the scan result.</summary>
    public int DetectedFoodId { get; } = DetectedFoodId;

    /// <summary>Quantity of food to log in grams (g).</summary>
    public decimal QuantityG { get; } = QuantityG;

    /// <summary>Type of meal. Valid values: breakfast, lunch, dinner, snack.</summary>
    public string MealType { get; } = MealType;

    /// <summary>Date of the meal in ISO 8601 format (yyyy-MM-dd).</summary>
    public string Date { get; } = Date;
}
