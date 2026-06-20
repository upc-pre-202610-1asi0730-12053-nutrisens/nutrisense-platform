namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record LogMealResource(
    int UserId,
    int FoodId,
    string MealType,
    string Date,
    decimal QuantityG,
    string Source)
{
    /// <summary>User ID of the person logging the meal.</summary>
    public int UserId { get; } = UserId;

    /// <summary>Food ID from the food catalog.</summary>
    public int FoodId { get; } = FoodId;

    /// <summary>Type of meal. Valid values: breakfast, lunch, dinner, snack.</summary>
    public string MealType { get; } = MealType;

    /// <summary>Date of the meal in ISO 8601 format (yyyy-MM-dd).</summary>
    public string Date { get; } = Date;

    /// <summary>Quantity of food consumed in grams (g).</summary>
    public decimal QuantityG { get; } = QuantityG;

    /// <summary>Source of the log entry (e.g., manual, scan, app). Valid values: manual, scan, app.</summary>
    public string Source { get; } = Source;
}
