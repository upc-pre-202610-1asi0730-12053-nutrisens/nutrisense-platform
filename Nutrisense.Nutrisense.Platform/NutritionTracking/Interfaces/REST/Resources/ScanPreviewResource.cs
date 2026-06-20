namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record ScannedDishItemResource(
    int? FoodId,
    string NameEn,
    string NameEs,
    decimal EstimatedQuantityG,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    bool IsEstimate)
{
    /// <summary>Food ID matched from the catalog. Null if no match found.</summary>
    public int? FoodId { get; } = FoodId;

    /// <summary>Detected food name in English.</summary>
    public string NameEn { get; } = NameEn;

    /// <summary>Detected food name in Spanish.</summary>
    public string NameEs { get; } = NameEs;

    /// <summary>Estimated quantity of food detected in the photo (g).</summary>
    public decimal EstimatedQuantityG { get; } = EstimatedQuantityG;

    /// <summary>Calorie content per 100 grams (kcal).</summary>
    public decimal CaloriesPer100g { get; } = CaloriesPer100g;

    /// <summary>Protein content per 100 grams (g).</summary>
    public decimal ProteinPer100g { get; } = ProteinPer100g;

    /// <summary>Carbohydrate content per 100 grams (g).</summary>
    public decimal CarbsPer100g { get; } = CarbsPer100g;

    /// <summary>Fat content per 100 grams (g).</summary>
    public decimal FatPer100g { get; } = FatPer100g;

    /// <summary>Indicates whether this is an estimated result from AI detection (true) or a confirmed match (false).</summary>
    public bool IsEstimate { get; } = IsEstimate;
}

public record ScanPreviewResource(IEnumerable<ScannedDishItemResource> Items)
{
    /// <summary>List of food items detected from the scanned dish photo.</summary>
    public IEnumerable<ScannedDishItemResource> Items { get; } = Items;
}
