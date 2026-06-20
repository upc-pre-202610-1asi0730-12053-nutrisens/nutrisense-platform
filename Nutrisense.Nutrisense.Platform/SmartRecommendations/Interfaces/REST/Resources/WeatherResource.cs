namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Current weather conditions for a specific city.</summary>
public record WeatherResource(
    /// <summary>City ID for which the weather is reported.</summary>
    int CityId,
    /// <summary>Temperature in degrees Celsius.</summary>
    decimal TempC,
    /// <summary>Weather condition description (e.g., "Clear", "Rainy", "Cloudy").</summary>
    string Condition,
    /// <summary>Weather type category (e.g., "clear", "rain", "cloud", "snow"). Valid values: clear, rain, cloud, snow, storm, fog.</summary>
    string WeatherType,
    /// <summary>Timestamp of weather observation in UTC (ISO 8601 format: yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    DateTimeOffset At);
