using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Seeders;

public class RecipesSeeder(AppDbContext context)
{
    public async Task SeedAsync()
    {
        if (await context.Recipes.AnyAsync()) return;

        var ingredientIds = await context.IngredientCatalogItems
            .Select(i => new { i.Key, i.Id })
            .ToDictionaryAsync(i => i.Key, i => i.Id);

        int Ingredient(string key) => ingredientIds.TryGetValue(key, out var id) ? id : 0;

        var recipes = new List<Recipe>
        {
            // Weight-loss recipes
            new("grilled-chicken-broccoli", "Grilled Chicken & Broccoli", "Pollo a la Plancha con Brócoli",
                "weight-loss", 20, 1, 310m, 35m, 12m, 8m, 4m, []),
            new("oat-banana-smoothie", "Oat & Banana Smoothie", "Batido de Avena y Plátano",
                "weight-loss", 5, 1, 280m, 10m, 52m, 4m, 5m, []),
            new("salmon-salad", "Salmon Salad", "Ensalada de Salmón",
                "weight-loss", 15, 1, 340m, 28m, 8m, 20m, 3m, ["seafood"]),
            new("lentil-soup", "Lentil Soup", "Sopa de Lentejas",
                "weight-loss", 30, 2, 250m, 18m, 38m, 3m, 10m, []),
            new("brown-rice-broccoli", "Brown Rice & Broccoli Bowl", "Tazón de Arroz Integral con Brócoli",
                "weight-loss", 25, 1, 295m, 9m, 58m, 4m, 6m, []),

            // Muscle-gain recipes
            new("chicken-rice-bowl", "Chicken & Rice Bowl", "Tazón de Pollo con Arroz",
                "muscle-gain", 30, 1, 520m, 48m, 52m, 10m, 4m, []),
            new("almond-oat-parfait", "Almond & Oat Parfait", "Parfait de Almendras y Avena",
                "muscle-gain", 10, 1, 480m, 18m, 52m, 22m, 8m, ["nuts"]),
            new("salmon-rice", "Salmon & Brown Rice", "Salmón con Arroz Integral",
                "muscle-gain", 25, 1, 560m, 40m, 48m, 18m, 3m, ["seafood"]),
            new("milk-oat-bowl", "Whole Milk Oat Bowl", "Tazón de Avena con Leche Entera",
                "muscle-gain", 10, 1, 420m, 16m, 58m, 12m, 6m, ["lactose"]),
            new("chicken-almond-salad", "Chicken & Almond Salad", "Ensalada de Pollo y Almendras",
                "muscle-gain", 15, 1, 490m, 42m, 14m, 28m, 4m, ["nuts"])
        };

        await context.Recipes.AddRangeAsync(recipes);
        await context.SaveChangesAsync();

        // Seed recipe ingredients
        var savedRecipes = await context.Recipes.ToDictionaryAsync(r => r.Key, r => r.Id);

        var ingredients = new List<RecipeIngredientItem>();

        void Add(string recipeKey, string ingredientKey, decimal qty, string unit)
        {
            if (savedRecipes.TryGetValue(recipeKey, out var recipeId) && ingredientIds.ContainsKey(ingredientKey))
                ingredients.Add(new RecipeIngredientItem(recipeId, Ingredient(ingredientKey), qty, unit));
        }

        Add("grilled-chicken-broccoli", "chicken-breast", 150m, "g");
        Add("grilled-chicken-broccoli", "broccoli", 100m, "g");
        Add("oat-banana-smoothie", "oats", 40m, "g");
        Add("oat-banana-smoothie", "banana", 1m, "unit");
        Add("salmon-salad", "salmon", 150m, "g");
        Add("salmon-salad", "broccoli", 80m, "g");
        Add("lentil-soup", "lentils", 100m, "g");
        Add("brown-rice-broccoli", "brown-rice", 100m, "g");
        Add("brown-rice-broccoli", "broccoli", 100m, "g");
        Add("chicken-rice-bowl", "chicken-breast", 200m, "g");
        Add("chicken-rice-bowl", "brown-rice", 150m, "g");
        Add("almond-oat-parfait", "almonds", 30m, "g");
        Add("almond-oat-parfait", "oats", 50m, "g");
        Add("salmon-rice", "salmon", 150m, "g");
        Add("salmon-rice", "brown-rice", 150m, "g");
        Add("milk-oat-bowl", "whole-milk", 240m, "ml");
        Add("milk-oat-bowl", "oats", 50m, "g");
        Add("chicken-almond-salad", "chicken-breast", 150m, "g");
        Add("chicken-almond-salad", "almonds", 30m, "g");

        if (ingredients.Count > 0)
        {
            await context.RecipeIngredientItems.AddRangeAsync(ingredients);
            await context.SaveChangesAsync();
        }
    }
}
