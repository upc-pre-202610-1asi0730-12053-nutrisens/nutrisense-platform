namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>A city in the catalog with geographic and timezone information.</summary>
public record CityResource(
    /// <summary>Unique identifier for the city.</summary>
    int Id,
    /// <summary>Unique key/slug for the city (e.g., "new-york", "san-francisco").</summary>
    string Key,
    /// <summary>City name in English.</summary>
    string NameEn,
    /// <summary>City name in Spanish.</summary>
    string NameEs,
    /// <summary>Country name where the city is located.</summary>
    string Country,
    /// <summary>Latitude coordinate in decimal degrees, range: -90 to 90.</summary>
    decimal Lat,
    /// <summary>Longitude coordinate in decimal degrees, range: -180 to 180.</summary>
    decimal Lng,
    /// <summary>Timezone identifier (e.g., "America/New_York", "Europe/Madrid").</summary>
    string Timezone);
