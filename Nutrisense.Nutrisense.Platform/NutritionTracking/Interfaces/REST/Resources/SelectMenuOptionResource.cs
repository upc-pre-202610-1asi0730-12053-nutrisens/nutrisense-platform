namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record SelectMenuOptionResource(
    int UserId,
    int FoodId,
    string MealType,
    string Date,
    decimal QuantityG)
{
    /// <summary>User ID of the person selecting the menu option.</summary>
    public int UserId { get; } = UserId;

    /// <summary>Food ID of the selected menu option.</summary>
    public int FoodId { get; } = FoodId;

    /// <summary>Type of meal. Valid values: breakfast, lunch, dinner, snack.</summary>
    public string MealType { get; } = MealType;

    /// <summary>Date of the meal in ISO 8601 format (yyyy-MM-dd).</summary>
    public string Date { get; } = Date;

    /// <summary>Quantity of food to log in grams (g).</summary>
    public decimal QuantityG { get; } = QuantityG;
}
