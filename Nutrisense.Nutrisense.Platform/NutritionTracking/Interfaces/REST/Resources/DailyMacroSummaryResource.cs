namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record DailyMacroSummaryResource(
    string Date,
    decimal TotalCalories,
    decimal TotalProteinG,
    decimal TotalCarbsG,
    decimal TotalFatG,
    decimal TotalFiberG,
    int MealCount)
{
    /// <summary>Date of the summary in ISO 8601 format (yyyy-MM-dd).</summary>
    public string Date { get; } = Date;

    /// <summary>Total calories consumed in this day (kcal).</summary>
    public decimal TotalCalories { get; } = TotalCalories;

    /// <summary>Total protein consumed in this day (g).</summary>
    public decimal TotalProteinG { get; } = TotalProteinG;

    /// <summary>Total carbohydrates consumed in this day (g).</summary>
    public decimal TotalCarbsG { get; } = TotalCarbsG;

    /// <summary>Total fat consumed in this day (g).</summary>
    public decimal TotalFatG { get; } = TotalFatG;

    /// <summary>Total dietary fiber consumed in this day (g).</summary>
    public decimal TotalFiberG { get; } = TotalFiberG;

    /// <summary>Number of meals logged in this day.</summary>
    public int MealCount { get; } = MealCount;
}
