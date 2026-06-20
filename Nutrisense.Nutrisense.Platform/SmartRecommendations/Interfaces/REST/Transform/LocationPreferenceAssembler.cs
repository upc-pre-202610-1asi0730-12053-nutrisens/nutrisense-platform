using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;

public static class LocationPreferenceAssembler
{
    public static LocationPreferenceResource ToResource(LocationPreference lp) =>
        new(lp.Id, lp.UserId, lp.HomeCityId, lp.CurrentCityId,
            lp.TravelModeActive, lp.IsManualOverride, lp.LocationPermissionGranted);
}
