namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

/// <summary>
/// Full, AI-estimated nutritional profile for a food that is NOT yet in the local catalog:
/// clean English/Spanish names, a coarse category, serving info, per-100g macros and the
/// dietary conflict tags it contains. Used to materialize a new <c>Food</c> on the fly during
/// a smart scan when no catalog match exists.
/// </summary>
public record EstimatedFoodData(
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
    IReadOnlyList<string> Restrictions);

/// <summary>
/// Port that estimates complete nutritional data for raw food names in batch (e.g. via an LLM).
/// Implemented in Infrastructure/External. Implementations MUST degrade gracefully (return
/// sensible defaults per name) on any failure so a scan never breaks on an LLM outage.
/// </summary>
public interface IFoodNutritionEstimationService
{
    Task<IReadOnlyList<EstimatedFoodData>> EstimateBatchAsync(
        IReadOnlyList<string> namesEn, CancellationToken ct = default);
}
