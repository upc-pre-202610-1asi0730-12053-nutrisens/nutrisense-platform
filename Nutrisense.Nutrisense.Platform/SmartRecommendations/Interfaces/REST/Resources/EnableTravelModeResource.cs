namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to enable travel mode for a user in a specific city.</summary>
public record EnableTravelModeResource(
    /// <summary>City ID to activate travel mode for.</summary>
    int CityId);
