namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

/// <summary>
/// Raw nutritional data for a single food as returned by an external catalog
/// (e.g. USDA FoodData Central), normalized to per-100g macros. Names are in the
/// source language (English) and not yet translated/enriched.
/// </summary>
public record ExternalFoodData(
    string? ExternalId,
    string Name,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    decimal FiberPer100g,
    decimal SugarPer100g,
    decimal ServingSize,
    string ServingUnit,
    string Source);

/// <summary>Port to an external food catalog. Implemented in Infrastructure/External.</summary>
public interface IExternalFoodDataProvider
{
    Task<IReadOnlyList<ExternalFoodData>> SearchAsync(
        string query, int maxResults, string dataType, CancellationToken ct = default);
}
