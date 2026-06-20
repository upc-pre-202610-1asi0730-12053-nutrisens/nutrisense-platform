namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal;

/// <summary>
/// A city search hit. <see cref="Id"/> is non-null when the city already exists in the catalog;
/// null for a geocoding-only candidate that has not been imported yet.
/// </summary>
public record CitySearchResult(
    int? Id,
    string? Key,
    string NameEn,
    string NameEs,
    string Country,
    string? State,
    decimal Lat,
    decimal Lng);
