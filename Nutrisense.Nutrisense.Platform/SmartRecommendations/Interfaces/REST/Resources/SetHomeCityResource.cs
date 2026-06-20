namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to set a user's home city.</summary>
public record SetHomeCityResource(
    /// <summary>City ID to set as the user's home city.</summary>
    int CityId);
