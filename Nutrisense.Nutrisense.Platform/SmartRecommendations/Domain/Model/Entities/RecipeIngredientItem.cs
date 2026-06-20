namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;

public class RecipeIngredientItem
{
    public int Id { get; private set; }
    public int RecipeId { get; private set; }
    public int IngredientCatalogItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;

    protected RecipeIngredientItem() { }

    public RecipeIngredientItem(int recipeId, int ingredientCatalogItemId, decimal quantity, string unit)
    {
        RecipeId = recipeId;
        IngredientCatalogItemId = ingredientCatalogItemId;
        Quantity = quantity;
        Unit = unit;
    }

    /// <summary>
    /// Creates a line owned by a <see cref="Aggregates.Recipe"/> aggregate; <see cref="RecipeId"/> is
    /// filled by EF from the relationship when the recipe is persisted.
    /// </summary>
    public RecipeIngredientItem(int ingredientCatalogItemId, decimal quantity, string unit)
    {
        IngredientCatalogItemId = ingredientCatalogItemId;
        Quantity = quantity;
        Unit = unit;
    }
}
