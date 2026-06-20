using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Persistence.EFC.Seeders;

public class FoodSeeder(AppDbContext context)
{
    public async Task SeedAsync()
    {
        if (await context.Foods.AnyAsync()) return;

        var foods = new List<Food>
        {
            new(new RegisterFoodCommand("oatmeal", "avena", "Grain", "manual", null, 40m, "g",
                389m, 16.9m, 66.3m, 6.9m, 10.6m, 1.1m, [])),
            new(new RegisterFoodCommand("chicken breast", "pechuga de pollo", "Protein", "manual", null, 120m, "g",
                165m, 31.0m, 0m, 3.6m, 0m, 0m, [])),
            new(new RegisterFoodCommand("brown rice", "arroz integral", "Grain", "manual", null, 100m, "g",
                216m, 4.5m, 44.8m, 1.8m, 1.8m, 0.7m, [])),
            new(new RegisterFoodCommand("banana", "plátano", "Fruit", "manual", null, 118m, "unit",
                89m, 1.1m, 22.8m, 0.3m, 2.6m, 12.2m, [])),
            new(new RegisterFoodCommand("egg", "huevo", "Protein", "manual", null, 60m, "unit",
                155m, 12.6m, 1.1m, 10.6m, 0m, 1.1m, [])),
            new(new RegisterFoodCommand("salmon", "salmón", "Protein", "manual", null, 150m, "g",
                208m, 20.4m, 0m, 13.4m, 0m, 0m, ["seafood"])),
            new(new RegisterFoodCommand("broccoli", "brócoli", "Vegetable", "manual", null, 100m, "g",
                34m, 2.8m, 6.6m, 0.4m, 2.6m, 1.7m, [])),
            new(new RegisterFoodCommand("whole milk", "leche entera", "Dairy", "manual", null, 240m, "ml",
                61m, 3.2m, 4.8m, 3.3m, 0m, 4.8m, ["lactose"])),
            new(new RegisterFoodCommand("almonds", "almendras", "Fat", "manual", null, 30m, "g",
                579m, 21.2m, 21.6m, 49.9m, 12.5m, 4.4m, ["nuts"])),
            new(new RegisterFoodCommand("sweet potato", "camote", "Vegetable", "manual", null, 130m, "g",
                86m, 1.6m, 20.1m, 0.1m, 3.0m, 4.2m, []))
        };

        await context.Foods.AddRangeAsync(foods);
        await context.SaveChangesAsync();
    }
}
