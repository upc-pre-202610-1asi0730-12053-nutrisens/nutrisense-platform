namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to update quantity and unit for a pantry item.</summary>
public record UpdatePantryItemResource(
    /// <summary>New quantity of the item.</summary>
    decimal Quantity,
    /// <summary>Unit of measurement (e.g., "g", "ml", "cup", "tbsp").</summary>
    string Unit);
