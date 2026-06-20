namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>
/// A city search result. <c>Id</c> is null for geocoding candidates not yet imported into the catalog;
/// the client persists them via POST /cities before using them as home/current/travel city.
/// </summary>
public record CitySearchResultResource(
    /// <summary>Catalog ID if already imported, or null if this is a geocoding candidate.</summary>
    int? Id,
    /// <summary>Unique key/slug if in catalog, or null for geocoding candidates.</summary>
    string? Key,
    /// <summary>City name in English.</summary>
    string NameEn,
    /// <summary>City name in Spanish.</summary>
    string NameEs,
    /// <summary>Country name where the city is located.</summary>
    string Country,
    /// <summary>State or region name, or null if not applicable (e.g., outside the US).</summary>
    string? State,
    /// <summary>Latitude coordinate in decimal degrees, range: -90 to 90.</summary>
    decimal Lat,
    /// <summary>Longitude coordinate in decimal degrees, range: -180 to 180.</summary>
    decimal Lng);
