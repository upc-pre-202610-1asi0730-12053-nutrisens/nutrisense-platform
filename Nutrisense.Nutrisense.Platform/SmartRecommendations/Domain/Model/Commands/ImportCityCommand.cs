namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

/// <summary>
/// Imports (or reuses, if already present) a city resolved from geocoding into the local catalog.
/// </summary>
public record ImportCityCommand(
    string Name,
    string? NameEn,
    string? NameEs,
    string Country,
    decimal Lat,
    decimal Lng);
