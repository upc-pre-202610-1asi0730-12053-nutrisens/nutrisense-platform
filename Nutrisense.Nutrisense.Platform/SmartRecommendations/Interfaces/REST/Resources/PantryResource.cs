namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>A user's pantry containing available ingredients.</summary>
public record PantryResource(
    /// <summary>Unique identifier for the pantry.</summary>
    int Id,
    /// <summary>User ID this pantry belongs to.</summary>
    int UserId,
    /// <summary>Collection of items in the pantry.</summary>
    IEnumerable<PantryItemResource> Items);
