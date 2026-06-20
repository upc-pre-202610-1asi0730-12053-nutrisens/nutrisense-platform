using System.Text.Json.Serialization;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External;

/// <summary>
/// Partial mapping of the OpenWeatherMap "Current weather data" response.
/// Only the fields consumed by <see cref="OpenWeatherMapWeatherService"/> are declared.
/// </summary>
public record OpenWeatherMapResponse
{
    [JsonPropertyName("weather")]
    public List<WeatherEntry> Weather { get; init; } = [];

    [JsonPropertyName("main")]
    public MainBlock? Main { get; init; }

    public record WeatherEntry
    {
        [JsonPropertyName("main")]
        public string? Main { get; init; }

        [JsonPropertyName("description")]
        public string? Description { get; init; }
    }

    public record MainBlock
    {
        [JsonPropertyName("temp")]
        public decimal Temp { get; init; }
    }
}
