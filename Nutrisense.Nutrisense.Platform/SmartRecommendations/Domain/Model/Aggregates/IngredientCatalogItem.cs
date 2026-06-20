namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

public class IngredientCatalogItem
{
    public int Id { get; private set; }
    public string Key { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string NameEs { get; private set; } = null!;

    /// <summary>Reference to NutritionTracking.Food — no FK, cross-BC boundary.</summary>
    public int? FoodId { get; private set; }

    public string Category { get; private set; } = null!;
    public string DefaultUnit { get; private set; } = null!;
    public decimal? GramsPerUnit { get; private set; }

    protected IngredientCatalogItem() { }

    public IngredientCatalogItem(
        string key, string nameEn, string nameEs, int? foodId,
        string category, string defaultUnit, decimal? gramsPerUnit)
    {
        Key = key;
        NameEn = nameEn;
        NameEs = nameEs;
        FoodId = foodId;
        Category = category;
        DefaultUnit = defaultUnit;
        GramsPerUnit = gramsPerUnit;
    }
}
