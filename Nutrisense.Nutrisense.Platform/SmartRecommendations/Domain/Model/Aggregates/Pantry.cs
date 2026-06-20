using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

public partial class Pantry
{
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    private readonly List<PantryItem> _items = [];
    public IReadOnlyCollection<PantryItem> Items => _items.AsReadOnly();

    protected Pantry() { }

    public Pantry(int userId)
    {
        UserId = userId;
    }

    public void AddItems(IEnumerable<PantryItemInput> inputs)
    {
        foreach (var input in inputs)
            _items.Add(new PantryItem(Id, input.IngredientCatalogItemId, input.Quantity, input.Unit));
    }

    public void RemoveItem(int pantryItemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == pantryItemId)
            ?? throw new InvalidOperationException($"PantryItem {pantryItemId} not found.");
        _items.Remove(item);
    }

    public void UpdateItem(int pantryItemId, decimal quantity, string unit)
    {
        var item = _items.FirstOrDefault(i => i.Id == pantryItemId)
            ?? throw new InvalidOperationException($"PantryItem {pantryItemId} not found.");
        item.UpdateQuantity(quantity, unit);
    }
}
