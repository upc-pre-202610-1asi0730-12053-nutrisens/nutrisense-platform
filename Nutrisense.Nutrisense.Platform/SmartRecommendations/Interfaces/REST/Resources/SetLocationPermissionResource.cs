namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to persist the user's location-permission intent (account-level).</summary>
public record SetLocationPermissionResource(
    /// <summary>Whether the user has granted permission to use their location.</summary>
    bool Granted);
