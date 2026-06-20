namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Input data for adding an item to a user's pantry.</summary>
public record PantryItemInputResource(
    /// <summary>Reference to the ingredient in the catalog.</summary>
    int IngredientCatalogItemId,
    /// <summary>Quantity of the item in the specified unit.</summary>
    decimal Quantity,
    /// <summary>Unit of measurement (e.g., "g", "ml", "cup", "tbsp").</summary>
    string Unit);
