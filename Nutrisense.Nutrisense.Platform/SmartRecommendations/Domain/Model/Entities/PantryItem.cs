namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;

public class PantryItem
{
    public int Id { get; private set; }
    public int PantryId { get; private set; }
    public int IngredientCatalogItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public DateTimeOffset? ExpiresAt { get; private set; }

    protected PantryItem() { }

    public PantryItem(int pantryId, int ingredientCatalogItemId, decimal quantity, string unit)
    {
        PantryId = pantryId;
        IngredientCatalogItemId = ingredientCatalogItemId;
        Quantity = quantity;
        Unit = unit;
    }

    public void UpdateQuantity(decimal quantity, string unit)
    {
        Quantity = quantity;
        Unit = unit;
    }
}
