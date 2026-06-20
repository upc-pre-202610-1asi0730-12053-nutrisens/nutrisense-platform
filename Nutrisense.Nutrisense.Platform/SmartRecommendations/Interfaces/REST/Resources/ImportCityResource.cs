namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to import a geocoded city into the catalog.</summary>
public record ImportCityResource(
    /// <summary>Primary city name (typically in local language).</summary>
    string Name,
    /// <summary>City name in English, or null to use Name for English.</summary>
    string? NameEn,
    /// <summary>City name in Spanish, or null to use Name for Spanish.</summary>
    string? NameEs,
    /// <summary>Country name.</summary>
    string Country,
    /// <summary>Latitude coordinate in decimal degrees, range: -90 to 90.</summary>
    decimal Lat,
    /// <summary>Longitude coordinate in decimal degrees, range: -180 to 180.</summary>
    decimal Lng);
