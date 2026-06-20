using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External;

/// <summary>
/// Resolves the current weather for a city via the OpenWeatherMap "Current weather data" API,
/// keyed by the domain <c>City</c> coordinates. Results are cached in memory to respect API
/// rate limits, and any failure degrades gracefully to a neutral snapshot so recommendation
/// generation never breaks on a weather outage.
/// </summary>
public class OpenWeatherMapWeatherService(
    HttpClient httpClient,
    ICityRepository cityRepository,
    IMemoryCache cache,
    IConfiguration configuration,
    ILogger<OpenWeatherMapWeatherService> logger) : IWeatherService
{
    private static readonly WeatherSnapshot Fallback =
        new(20m, "partly-cloudy", "warm", DateTimeOffset.UtcNow);

    public async Task<WeatherSnapshot> GetCurrentAsync(int cityId, CancellationToken ct = default)
    {
        var cacheMinutes = int.TryParse(configuration["OpenWeatherMap:CacheMinutes"], out var m) ? m : 10;

        if (cache.TryGetValue($"weather:{cityId}", out WeatherSnapshot? cached) && cached is not null)
            return cached;

        var apiKey = configuration["OpenWeatherMap:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("OpenWeatherMap API key not configured; returning fallback weather for city {CityId}.", cityId);
            return Fallback with { At = DateTimeOffset.UtcNow };
        }

        var city = await cityRepository.FindByIdAsync(cityId, ct);
        if (city is null)
        {
            logger.LogWarning("City {CityId} not found while resolving weather; returning fallback.", cityId);
            return Fallback with { At = DateTimeOffset.UtcNow };
        }

        try
        {
            var baseUrl = configuration["OpenWeatherMap:BaseUrl"]?.TrimEnd('/')
                          ?? "https://api.openweathermap.org/data/2.5";
            var lat = city.Lat.ToString(CultureInfo.InvariantCulture);
            var lng = city.Lng.ToString(CultureInfo.InvariantCulture);
            var url = $"{baseUrl}/weather?lat={lat}&lon={lng}&units=metric&appid={apiKey}";

            var response = await httpClient.GetFromJsonAsync<OpenWeatherMapResponse>(url, ct);
            if (response?.Main is null)
            {
                logger.LogWarning("Empty OpenWeatherMap response for city {CityId}; returning fallback.", cityId);
                return Fallback with { At = DateTimeOffset.UtcNow };
            }

            var tempC = Math.Round(response.Main.Temp, 1);
            var owmGroup = response.Weather.FirstOrDefault()?.Main;
            var snapshot = new WeatherSnapshot(
                tempC,
                MapCondition(owmGroup),
                MapWeatherType(tempC),
                DateTimeOffset.UtcNow);

            cache.Set($"weather:{cityId}", snapshot, TimeSpan.FromMinutes(cacheMinutes));
            return snapshot;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching weather for city {CityId} from OpenWeatherMap.", cityId);
            return Fallback with { At = DateTimeOffset.UtcNow };
        }
    }

    /// <summary>Maps OpenWeatherMap's condition group to the domain's allowed weather conditions.</summary>
    private static string MapCondition(string? owmGroup) => owmGroup switch
    {
        "Clear" => "sunny",
        "Clouds" => "cloudy",
        "Rain" or "Drizzle" or "Thunderstorm" => "rainy",
        _ => "partly-cloudy",
    };

    /// <summary>Buckets a temperature in °C into the domain's allowed weather types.</summary>
    private static string MapWeatherType(decimal tempC) => tempC switch
    {
        >= 28m => "hot",
        >= 20m => "warm",
        >= 12m => "mild",
        _ => "cold",
    };
}
