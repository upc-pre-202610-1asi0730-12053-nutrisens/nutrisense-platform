using System.Text.RegularExpressions;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.CommandServices;

/// <summary>
/// Derives the ingredient catalog from the imported food catalog: each food whose category maps
/// to a cooking-ingredient category becomes an <see cref="IngredientCatalogItem"/> linked back to
/// the food (by id), inheriting its translated names and category. Idempotent: dedups by key.
/// </summary>
public partial class IngredientCatalogImportCommandService(
    INutritionTrackingContextFacade nutritionTrackingFacade,
    IIngredientCatalogRepository ingredientRepository,
    IUnitOfWork unitOfWork,
    ILogger<IngredientCatalogImportCommandService> logger) : IIngredientCatalogImportCommandService
{
    // Maps NutritionTracking food categories to the allowed ingredient categories.
    // Beverage/Other are intentionally absent — they are not cooking ingredients and are skipped.
    private static readonly Dictionary<string, string> CategoryMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Grain"] = "grain",
        ["Protein"] = "protein",
        ["Dairy"] = "dairy",
        ["Vegetable"] = "vegetable",
        ["Fruit"] = "fruit",
        ["Fat"] = "fat",
        ["Legume"] = "legume"
    };

    public async Task<int> Handle(DeriveIngredientsFromFoodsCommand command, CancellationToken ct = default)
    {
        var foods = await nutritionTrackingFacade.GetFoodCatalog(ct);
        var existing = await ingredientRepository.ListAsync(ct);
        var keys = existing.Select(i => i.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var derived = 0;
        foreach (var food in foods)
        {
            if (!CategoryMap.TryGetValue(food.Category, out var category))
                continue;

            var key = Slug(food.NameEn);
            if (string.IsNullOrEmpty(key) || !keys.Add(key))
                continue; // dedup, and guard against two foods slugging to the same key

            var gramsPerUnit = string.Equals(food.ServingUnit, "unit", StringComparison.OrdinalIgnoreCase)
                ? food.ServingSizeG
                : (decimal?)null;

            await ingredientRepository.AddAsync(
                new IngredientCatalogItem(key, food.NameEn, food.NameEs, food.Id, category, food.ServingUnit, gramsPerUnit),
                ct);
            derived++;
        }

        if (derived > 0) await unitOfWork.CompleteAsync(ct);
        logger.LogInformation("[IngredientDerive] Derived {Count} ingredients from {FoodCount} foods", derived, foods.Count);
        return derived;
    }

    private static string Slug(string name) =>
        SlugRegex().Replace(name.Trim().ToLowerInvariant(), "-").Trim('-');

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex SlugRegex();
}
