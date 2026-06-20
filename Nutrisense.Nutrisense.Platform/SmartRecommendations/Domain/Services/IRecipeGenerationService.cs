namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

/// <summary>One ingredient line of a generated recipe, referencing an ingredient catalog key.</summary>
public record GeneratedRecipeIngredient(string IngredientKey, decimal Quantity, string Unit);

/// <summary>A recipe proposed by the generator from the available ingredient catalog.</summary>
public record GeneratedRecipeData(
    string NameEn,
    string NameEs,
    int PrepTimeMinutes,
    int Servings,
    decimal Calories,
    decimal ProteinG,
    decimal CarbsG,
    decimal FatG,
    decimal FiberG,
    IReadOnlyList<string> RestrictionsConflict,
    IReadOnlyList<GeneratedRecipeIngredient> Ingredients);

/// <summary>
/// Port that generates recipes for a goal from the available ingredient catalog
/// (keys grouped by category). Implemented in Infrastructure/External.
/// </summary>
public interface IRecipeGenerationService
{
    Task<IReadOnlyList<GeneratedRecipeData>> GenerateAsync(
        IReadOnlyDictionary<string, IReadOnlyList<string>> ingredientKeysByCategory,
        string goalType,
        int count,
        CancellationToken ct = default);
}
