using System.Text.Json.Serialization;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External;

/// <summary>
/// One entry of the OpenWeatherMap Geocoding API response (both direct and reverse return arrays of this shape).
/// </summary>
public record OpenWeatherMapGeoEntry
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("local_names")]
    public Dictionary<string, string>? LocalNames { get; init; }

    [JsonPropertyName("lat")]
    public decimal Lat { get; init; }

    [JsonPropertyName("lon")]
    public decimal Lon { get; init; }

    [JsonPropertyName("country")]
    public string Country { get; init; } = "";

    [JsonPropertyName("state")]
    public string? State { get; init; }
}
