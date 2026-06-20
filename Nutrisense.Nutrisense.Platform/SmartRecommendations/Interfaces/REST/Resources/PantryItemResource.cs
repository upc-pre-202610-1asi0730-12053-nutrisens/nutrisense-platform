namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>A pantry item representing an ingredient available to the user.</summary>
public record PantryItemResource(
    /// <summary>Unique identifier for the pantry item.</summary>
    int Id,
    /// <summary>Reference to the ingredient in the catalog.</summary>
    int IngredientCatalogItemId,
    /// <summary>Quantity of the item in the specified unit.</summary>
    decimal Quantity,
    /// <summary>Unit of measurement (e.g., "g", "ml", "cup", "tbsp").</summary>
    string Unit,
    /// <summary>Optional expiration date in UTC (ISO 8601 format: yyyy-MM-dd). Null if non-perishable.</summary>
    DateTimeOffset? ExpiresAt);
