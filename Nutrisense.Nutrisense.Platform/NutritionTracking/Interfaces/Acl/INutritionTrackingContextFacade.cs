namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;

/// <summary>
/// A catalog food, flattened to primitives for cross-BC consumption.
/// </summary>
public record FoodCatalogItem(
    int Id,
    string Key,
    string NameEn,
    string NameEs,
    string Category,
    string ServingUnit,
    decimal ServingSizeG);

/// <summary>
/// A food resolved against the catalog or freshly materialized, flattened to primitives.
/// </summary>
public record ProvisionedFoodItem(
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
/// A day's aggregated macros, flattened to primitives.
/// </summary>
public record DailyMacroSummaryItem(
    decimal TotalCalories,
    decimal TotalProteinG,
    decimal TotalCarbsG,
    decimal TotalFatG,
    decimal TotalFiberG);

/// <summary>
/// Public Anti-Corruption-Layer contract for the NutritionTracking bounded context.
/// Other BCs consume nutrition data through this facade without coupling to NutritionTracking's
/// domain model: every parameter and return value is a primitive or a primitive-only DTO defined
/// here — never a Command, aggregate or entity. Every method degrades gracefully, returning an
/// empty collection or null on failure instead of throwing.
/// </summary>
public interface INutritionTrackingContextFacade
{
    /// <summary>Returns the entire food catalog. Empty if none / on failure.</summary>
    Task<IReadOnlyList<FoodCatalogItem>> GetFoodCatalog(CancellationToken ct = default);

    /// <summary>
    /// Resolves a batch of raw English food names against the catalog, estimating and persisting
    /// any miss. Names that can neither be matched nor created are omitted. Empty on failure.
    /// </summary>
    Task<IReadOnlyList<ProvisionedFoodItem>> ResolveOrCreateFoodsByNames(
        IReadOnlyList<string> namesEn, string source, CancellationToken ct = default);

    /// <summary>Returns the day's aggregated macros for a user, or null if nothing was logged.</summary>
    Task<DailyMacroSummaryItem?> GetDailyMacroSummary(int userId, DateOnly date, CancellationToken ct = default);

    /// <summary>Returns the distinct meal types logged by a user on a date. Empty if none / on failure.</summary>
    Task<IReadOnlyList<string>> GetLoggedMealTypes(int userId, DateOnly date, CancellationToken ct = default);
}
