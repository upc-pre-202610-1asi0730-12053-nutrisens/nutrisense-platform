namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;

/// <summary>
/// A food resolved against the local catalog or freshly materialized (AI-estimated) from a raw name.
/// Carries everything a consumer needs to reference and describe the food without a second lookup.
/// </summary>
public record ProvisionedFood(
    int Id,
    string NameEn,
    string NameEs,
    decimal ServingSizeG,
    string ServingUnit,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    IReadOnlyList<string> Restrictions);

/// <summary>
/// Resolves a batch of raw food names into catalog <c>Food</c> rows: each name is matched against
/// the local catalog first, and any miss is estimated (via the LLM estimation port) and persisted as
/// a new food so it carries a real id. This is the same "search-first, estimate-the-rest" pattern the
/// smart scan uses, extracted so other contexts (e.g. SmartRecommendations) can reuse it without
/// duplicating the estimate-and-register dance. Degrades gracefully: names that can neither be matched
/// nor materialized are simply omitted from the result.
/// </summary>
public interface IFoodProvisioningService
{
    Task<IReadOnlyList<ProvisionedFood>> ResolveOrCreateByNamesAsync(
        IReadOnlyList<string> namesEn, string source, CancellationToken ct = default);
}
