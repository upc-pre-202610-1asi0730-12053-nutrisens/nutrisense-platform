namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to detect and set a user's location from geographic coordinates.</summary>
public record DetectLocationResource(
    /// <summary>Latitude coordinate in decimal degrees, range: -90 to 90.</summary>
    decimal Lat,
    /// <summary>Longitude coordinate in decimal degrees, range: -180 to 180.</summary>
    decimal Lng);
