namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal;

/// <summary>
/// A single dish read from a menu scan, resolved to a catalog food (matched or AI-materialized and
/// cached during the scan). <see cref="IsEstimate"/> flags dishes whose macros came from the LLM.
/// </summary>
public record ScannedMenuOption(
    int? FoodId,
    string NameEn,
    string NameEs,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    IReadOnlyList<string> Restrictions,
    bool IsEstimate);

/// <summary>
/// Preview of a menu scan. An empty <see cref="Options"/> list means "nothing detected" — the client
/// should show its menu fallback.
/// </summary>
public record MenuOptionsPreview(IReadOnlyList<ScannedMenuOption> Options);
