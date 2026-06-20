using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External;

/// <summary>
/// Resolves the nearest catalog city to a coordinate. If the closest known city is farther than
/// <see cref="MaxDistanceKm"/>, it reports failure so the caller can fall back to reverse-geocoding
/// and importing the real city instead of snapping to a far-away one.
/// </summary>
public class GeolocationService(ICityRepository cityRepository) : IGeolocationService
{
    private const double MaxDistanceKm = 100d;

    public async Task<GeoResult> DetectNearestCityAsync(decimal lat, decimal lng, CancellationToken ct = default)
    {
        var nearest = await cityRepository.FindNearestAsync(lat, lng, ct);
        if (nearest is null)
            return new GeoResult(false, null, "no-cities");

        var distance = HaversineKm(
            (double)lat, (double)lng, (double)nearest.Lat, (double)nearest.Lng);

        return distance <= MaxDistanceKm
            ? new GeoResult(true, nearest.Id, null)
            : new GeoResult(false, null, "too-far");
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371d;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180d;
}
