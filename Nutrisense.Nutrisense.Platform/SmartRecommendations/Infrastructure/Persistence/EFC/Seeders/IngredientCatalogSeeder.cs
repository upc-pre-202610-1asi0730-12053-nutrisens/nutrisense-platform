using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Seeders;

public class IngredientCatalogSeeder(AppDbContext context)
{
    public async Task SeedAsync()
    {
        if (await context.IngredientCatalogItems.AnyAsync()) return;

        var foodIds = await context.Foods
            .Select(f => new { f.Key, f.Id })
            .ToDictionaryAsync(f => f.Key, f => f.Id);

        int? FoodId(string key) => foodIds.TryGetValue(key, out var id) ? id : null;

        var items = new List<IngredientCatalogItem>
        {
            new("chicken-breast", "Chicken Breast", "Pechuga de Pollo",
                FoodId("manual-chicken-breast"), "protein", "g", 100m),
            new("oats", "Oats", "Avena",
                FoodId("manual-oatmeal"), "grain", "g", 100m),
            new("broccoli", "Broccoli", "Brócoli",
                FoodId("manual-broccoli"), "vegetable", "g", 100m),
            new("banana", "Banana", "Plátano",
                FoodId("manual-banana"), "fruit", "unit", 118m),
            new("olive-oil", "Olive Oil", "Aceite de Oliva",
                null, "fat", "ml", 0.92m),
            new("whole-milk", "Whole Milk", "Leche Entera",
                FoodId("manual-whole-milk"), "dairy", "ml", 1.03m),
            new("lentils", "Lentils", "Lentejas",
                null, "legume", "g", 100m),
            new("salmon", "Salmon", "Salmón",
                FoodId("manual-salmon"), "protein", "g", 100m),
            new("brown-rice", "Brown Rice", "Arroz Integral",
                FoodId("manual-brown-rice"), "grain", "g", 100m),
            new("almonds", "Almonds", "Almendras",
                FoodId("manual-almonds"), "fat", "g", 100m)
        };

        await context.IngredientCatalogItems.AddRangeAsync(items);
        await context.SaveChangesAsync();
    }
}
