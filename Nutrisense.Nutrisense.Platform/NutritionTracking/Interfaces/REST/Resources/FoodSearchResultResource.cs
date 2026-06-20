namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record FoodSearchResultResource(
    int Id,
    string NameEn,
    string NameEs,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    string Source,
    IEnumerable<string> Restrictions)
{
    /// <summary>Unique identifier of the food item.</summary>
    public int Id { get; } = Id;

    /// <summary>Food name in English.</summary>
    public string NameEn { get; } = NameEn;

    /// <summary>Food name in Spanish.</summary>
    public string NameEs { get; } = NameEs;

    /// <summary>Calorie content per 100 grams (kcal).</summary>
    public decimal CaloriesPer100g { get; } = CaloriesPer100g;

    /// <summary>Protein content per 100 grams (g).</summary>
    public decimal ProteinPer100g { get; } = ProteinPer100g;

    /// <summary>Carbohydrate content per 100 grams (g).</summary>
    public decimal CarbsPer100g { get; } = CarbsPer100g;

    /// <summary>Fat content per 100 grams (g).</summary>
    public decimal FatPer100g { get; } = FatPer100g;

    /// <summary>Data source of the food entry (e.g., USDA, local). Valid values: USDA, LOCAL.</summary>
    public string Source { get; } = Source;

    /// <summary>Array of dietary restrictions or allergens (e.g., gluten-free, vegan, nut-free).</summary>
    public IEnumerable<string> Restrictions { get; } = Restrictions;
}
