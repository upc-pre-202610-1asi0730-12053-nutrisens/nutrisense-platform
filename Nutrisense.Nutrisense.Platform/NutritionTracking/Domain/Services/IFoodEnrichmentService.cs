namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

/// <summary>
/// Enriched, human-friendly metadata for a food: a clean English name, its Spanish
/// translation, a coarse category and the dietary conflict tags it contains.
/// </summary>
public record EnrichedFoodData(
    string NameEn,
    string NameEs,
    string Category,
    IReadOnlyList<string> Restrictions);

/// <summary>
/// Port that translates and categorizes raw external food names in batch
/// (e.g. via an LLM). Implemented in Infrastructure/External.
/// </summary>
public interface IFoodEnrichmentService
{
    Task<IReadOnlyList<EnrichedFoodData>> EnrichBatchAsync(
        IReadOnlyList<string> namesEn, CancellationToken ct = default);
}
