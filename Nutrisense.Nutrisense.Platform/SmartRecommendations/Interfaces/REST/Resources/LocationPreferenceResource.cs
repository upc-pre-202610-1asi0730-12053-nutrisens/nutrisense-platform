namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>User's location preferences and travel mode settings.</summary>
public record LocationPreferenceResource(
    /// <summary>Unique identifier for the location preference record.</summary>
    int Id,
    /// <summary>User ID this location preference belongs to.</summary>
    int UserId,
    /// <summary>City ID set as home location, or null if not configured.</summary>
    int? HomeCityId,
    /// <summary>City ID of the user's current detected location, or null if unknown.</summary>
    int? CurrentCityId,
    /// <summary>Whether travel mode is currently enabled.</summary>
    bool TravelModeActive,
    /// <summary>Whether the current location is manually overridden by the user.</summary>
    bool IsManualOverride,
    /// <summary>Whether the user has granted location permission for automatic detection.</summary>
    bool LocationPermissionGranted);
