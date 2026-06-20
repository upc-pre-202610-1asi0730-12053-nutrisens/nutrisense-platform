using System.Text.RegularExpressions;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Errors;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.CommandServices;

/// <summary>
/// Generates recipe suggestions per goal from the derived ingredient catalog: groups available
/// ingredient keys by category, asks the generator (DeepSeek) for batches of recipes, keeps only
/// recipes whose ingredients exist in the catalog, dedups by key and persists them with their lines.
/// </summary>
public partial class RecipeImportCommandService(
    IIngredientCatalogRepository ingredientRepository,
    IRecipeRepository recipeRepository,
    IRecipeGenerationService recipeGenerationService,
    IUnitOfWork unitOfWork,
    ILogger<RecipeImportCommandService> logger) : IRecipeImportCommandService
{
    private const int BatchSize = 5;
    private const int MinIngredients = 3;
    private static readonly string[] DefaultGoals = ["weight-loss", "muscle-gain"];

    public async Task<Result<int, RecipeImportError>> Handle(
        ImportRecipeSuggestionsCommand command, CancellationToken ct = default)
    {
        try
        {
            var ingredients = (await ingredientRepository.ListAsync(ct)).ToList();
            if (ingredients.Count < MinIngredients)
            {
                logger.LogWarning("[RecipeImport] Only {Count} ingredients available; need at least {Min}.",
                    ingredients.Count, MinIngredients);
                return new Result<int, RecipeImportError>.Failure(RecipeImportError.InsufficientIngredients);
            }

            var keyToItem = ingredients
                .GroupBy(i => i.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var byCategory = ingredients
                .GroupBy(i => i.Category)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(i => i.Key).ToList());

            var existingKeys = (await recipeRepository.ListAsync(ct))
                .Select(r => r.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var goals = command.GoalTypes is { Count: > 0 } g2 ? g2 : DefaultGoals;
            var maxPerGoal = command.MaxPerGoal > 0 ? command.MaxPerGoal : 10;
            var total = 0;

            foreach (var goal in goals)
            {
                var generated = 0;
                while (generated < maxPerGoal)
                {
                    var count = Math.Min(BatchSize, maxPerGoal - generated);

                    IReadOnlyList<GeneratedRecipeData> batch;
                    try
                    {
                        batch = await recipeGenerationService.GenerateAsync(byCategory, goal, count, ct);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "[RecipeImport] Generation failed for goal={Goal}; skipping batch.", goal);
                        generated += count;
                        continue;
                    }

                    var saved = 0;
                    foreach (var data in batch)
                    {
                        var validLines = data.Ingredients
                            .Where(li => !string.IsNullOrWhiteSpace(li.IngredientKey) && keyToItem.ContainsKey(li.IngredientKey))
                            .ToList();
                        if (validLines.Count == 0)
                            continue;

                        var key = Slug(data.NameEn);
                        if (string.IsNullOrEmpty(key) || !existingKeys.Add(key))
                            continue;

                        var recipe = new Recipe(
                            key, data.NameEn, data.NameEs, goal,
                            data.PrepTimeMinutes, data.Servings <= 0 ? 1 : data.Servings,
                            data.Calories, data.ProteinG, data.CarbsG, data.FatG, data.FiberG,
                            data.RestrictionsConflict.Select(r => r.Trim().ToLowerInvariant()).ToList());

                        foreach (var line in validLines)
                        {
                            var item = keyToItem[line.IngredientKey];
                            var unit = string.IsNullOrWhiteSpace(line.Unit) ? item.DefaultUnit : line.Unit;
                            recipe.AddIngredient(item.Id, line.Quantity, unit);
                        }

                        await recipeRepository.AddAsync(recipe, ct);
                        saved++;
                    }

                    if (saved > 0) await unitOfWork.CompleteAsync(ct);
                    total += saved;
                    generated += count;

                    logger.LogInformation("[RecipeImport] goal={Goal}: saved {Saved} (running total {Total})", goal, saved, total);
                }
            }

            logger.LogInformation("[RecipeImport] Complete: {Total} recipes generated", total);
            return new Result<int, RecipeImportError>.Success(total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RecipeImport] Unexpected error generating recipes");
            return new Result<int, RecipeImportError>.Failure(RecipeImportError.UnexpectedError);
        }
    }

    private static string Slug(string name) =>
        SlugRegex().Replace(name.Trim().ToLowerInvariant(), "-").Trim('-');

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex SlugRegex();
}
