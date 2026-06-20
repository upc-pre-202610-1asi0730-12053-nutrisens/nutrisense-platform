using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;

public static class RecipeAssembler
{
    public static RecipeResource ToResource(Recipe recipe) =>
        new(recipe.Id, recipe.Key, recipe.NameEn, recipe.NameEs,
            recipe.GoalType, recipe.PrepTimeMinutes, recipe.Servings,
            recipe.TotalCalories, recipe.TotalProteinG, recipe.TotalCarbsG,
            recipe.TotalFatG, recipe.TotalFiberG, recipe.RestrictionsConflict,
            recipe.Ingredients.Select(i =>
                new RecipeIngredientResource(i.Id, i.IngredientCatalogItemId, i.Quantity, i.Unit)));
}
