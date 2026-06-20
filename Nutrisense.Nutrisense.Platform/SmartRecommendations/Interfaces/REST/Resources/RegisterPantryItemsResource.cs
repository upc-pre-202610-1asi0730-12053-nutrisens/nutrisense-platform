namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to register multiple items to a user's pantry.</summary>
public record RegisterPantryItemsResource(
    /// <summary>Array of items to add to the pantry.</summary>
    PantryItemInputResource[] Items);
