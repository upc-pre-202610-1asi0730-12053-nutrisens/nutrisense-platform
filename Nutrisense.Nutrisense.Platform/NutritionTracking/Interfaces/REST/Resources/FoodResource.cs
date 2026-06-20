namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record FoodResource(
    int Id,
    string Key,
    string Source,
    string? ExternalId,
    string NameEn,
    string NameEs,
    string Category,
    decimal ServingSizeG,
    string ServingUnit,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    decimal FiberPer100g,
    decimal SugarPer100g,
    IEnumerable<string> Restrictions)
{
    /// <summary>Unique identifier of the food item.</summary>
    public int Id { get; } = Id;

    /// <summary>Unique key identifier for the food item.</summary>
    public string Key { get; } = Key;

    /// <summary>Data source of the food entry (e.g., USDA, local). Valid values: USDA, LOCAL.</summary>
    public string Source { get; } = Source;

    /// <summary>External identifier from the source system (e.g., USDA FDC ID).</summary>
    public string? ExternalId { get; } = ExternalId;

    /// <summary>Food name in English.</summary>
    public string NameEn { get; } = NameEn;

    /// <summary>Food name in Spanish.</summary>
    public string NameEs { get; } = NameEs;

    /// <summary>Food category (e.g., Vegetables, Fruits, Proteins). Accepted values: Vegetables, Fruits, Proteins, Grains, Dairy, Oils, Condiments, Other.</summary>
    public string Category { get; } = Category;

    /// <summary>Standard serving size in grams (g).</summary>
    public decimal ServingSizeG { get; } = ServingSizeG;

    /// <summary>Unit of serving measurement (e.g., grams, cup, piece).</summary>
    public string ServingUnit { get; } = ServingUnit;

    /// <summary>Calorie content per 100 grams (kcal).</summary>
    public decimal CaloriesPer100g { get; } = CaloriesPer100g;

    /// <summary>Protein content per 100 grams (g).</summary>
    public decimal ProteinPer100g { get; } = ProteinPer100g;

    /// <summary>Carbohydrate content per 100 grams (g).</summary>
    public decimal CarbsPer100g { get; } = CarbsPer100g;

    /// <summary>Fat content per 100 grams (g).</summary>
    public decimal FatPer100g { get; } = FatPer100g;

    /// <summary>Dietary fiber content per 100 grams (g).</summary>
    public decimal FiberPer100g { get; } = FiberPer100g;

    /// <summary>Sugar content per 100 grams (g).</summary>
    public decimal SugarPer100g { get; } = SugarPer100g;

    /// <summary>Array of dietary restrictions or allergens (e.g., gluten-free, vegan, nut-free).</summary>
    public IEnumerable<string> Restrictions { get; } = Restrictions;
}
