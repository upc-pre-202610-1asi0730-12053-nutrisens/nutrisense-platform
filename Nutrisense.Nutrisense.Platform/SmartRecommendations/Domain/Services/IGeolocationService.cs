namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

public record GeoResult(bool Success, int? NearestCityId, string? FailureReason);

public interface IGeolocationService
{
    Task<GeoResult> DetectNearestCityAsync(decimal lat, decimal lng, CancellationToken ct = default);
}
