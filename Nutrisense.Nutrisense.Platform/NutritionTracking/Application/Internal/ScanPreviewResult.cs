namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal;

/// <summary>
/// A single item resolved from a dish scan. <see cref="FoodId"/> is always set because the food is
/// either matched in the catalog or materialized (and persisted) from an AI estimate during the scan.
/// <see cref="IsEstimate"/> flags items whose macros came from the LLM rather than the catalog.
/// </summary>
public record ScannedDishItem(
    int? FoodId,
    string NameEn,
    string NameEs,
    decimal EstimatedQuantityG,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    bool IsEstimate);

/// <summary>
/// Preview of a dish scan. An empty <see cref="Items"/> list means "nothing detected" — the client
/// should show its dish fallback.
/// </summary>
public record ScanPreviewResult(IReadOnlyList<ScannedDishItem> Items);
