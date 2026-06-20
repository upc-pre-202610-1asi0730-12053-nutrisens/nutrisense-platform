using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class FoodResourceAssembler
{
    public static FoodResource ToResource(Food food) =>
        new(food.Id, food.Key, food.Source, food.ExternalId,
            food.NameEn, food.NameEs, food.Category, food.ServingSizeG, food.ServingUnit,
            food.CaloriesPer100g, food.ProteinPer100g, food.CarbsPer100g,
            food.FatPer100g, food.FiberPer100g, food.SugarPer100g, food.Restrictions);

    public static FoodSearchResultResource ToSearchResource(Food food) =>
        new(food.Id, food.NameEn, food.NameEs,
            food.CaloriesPer100g, food.ProteinPer100g, food.CarbsPer100g,
            food.FatPer100g, food.Source, food.Restrictions);
}
