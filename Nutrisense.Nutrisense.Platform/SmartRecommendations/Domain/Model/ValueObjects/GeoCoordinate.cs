namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.ValueObjects;

public sealed record GeoCoordinate(decimal Lat, decimal Lng)
{
    public decimal Lat { get; } = Lat is >= -90 and <= 90
        ? Lat
        : throw new ArgumentException($"Latitude must be in [-90, 90]: {Lat}");

    public decimal Lng { get; } = Lng is >= -180 and <= 180
        ? Lng
        : throw new ArgumentException($"Longitude must be in [-180, 180]: {Lng}");
}
