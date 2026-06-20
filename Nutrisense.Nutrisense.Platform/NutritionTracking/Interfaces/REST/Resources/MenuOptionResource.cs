namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record MenuOptionResource(
    int? FoodId,
    string NameEn,
    string NameEs,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    IEnumerable<string> Restrictions,
    bool IsEstimate)
{
    /// <summary>Food ID matched from the catalog. Null if no match found.</summary>
    public int? FoodId { get; } = FoodId;

    /// <summary>Menu option name in English.</summary>
    public string NameEn { get; } = NameEn;

    /// <summary>Menu option name in Spanish.</summary>
    public string NameEs { get; } = NameEs;

    /// <summary>Calorie content per 100 grams (kcal).</summary>
    public decimal CaloriesPer100g { get; } = CaloriesPer100g;

    /// <summary>Protein content per 100 grams (g).</summary>
    public decimal ProteinPer100g { get; } = ProteinPer100g;

    /// <summary>Carbohydrate content per 100 grams (g).</summary>
    public decimal CarbsPer100g { get; } = CarbsPer100g;

    /// <summary>Fat content per 100 grams (g).</summary>
    public decimal FatPer100g { get; } = FatPer100g;

    /// <summary>Array of dietary restrictions or allergens (e.g., gluten-free, vegan, nut-free).</summary>
    public IEnumerable<string> Restrictions { get; } = Restrictions;

    /// <summary>Indicates whether this is an estimated result from AI detection (true) or a confirmed menu item (false).</summary>
    public bool IsEstimate { get; } = IsEstimate;
}
