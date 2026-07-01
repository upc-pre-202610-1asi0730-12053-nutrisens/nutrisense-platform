namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

public partial class LocationPreference
{
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    public int? HomeCityId { get; private set; }
    public int? CurrentCityId { get; private set; }
    public bool TravelModeActive { get; private set; }
    public bool IsManualOverride { get; private set; }
    public bool LocationPermissionGranted { get; private set; }

    protected LocationPreference() { }

    public LocationPreference(int userId)
    {
        UserId = userId;
        TravelModeActive = false;
        IsManualOverride = false;
        LocationPermissionGranted = false;
    }

    public void EnableTravelMode(int cityId)
    {
        CurrentCityId = cityId;
        TravelModeActive = true;
    }

    /// <summary>
    /// Turns travel mode off and reverts the active city to the user's home city. Travel mode overwrites
    /// <see cref="CurrentCityId"/> with the visited city, so on the way back home we restore it from
    /// <see cref="HomeCityId"/>. If no home city is set there is nothing to revert to, so the visited
    /// city is left in place.
    /// </summary>
    public void DisableTravelMode()
    {
        TravelModeActive = false;
        if (HomeCityId is not null)
            CurrentCityId = HomeCityId;
    }

    public void DetectLocation(int cityId)
    {
        CurrentCityId = cityId;
        IsManualOverride = false;
    }

    public void SetHomeCity(int cityId)
    {
        HomeCityId = cityId;
    }

    /// <summary>
    /// Records the user's intent to use location-based recommendations (account-level, persists
    /// across devices). The runtime ability to read GPS still depends on the browser permission;
    /// this flag only drives UX decisions such as whether to prompt again.
    /// </summary>
    public void SetLocationPermission(bool granted)
    {
        LocationPermissionGranted = granted;
    }
}
