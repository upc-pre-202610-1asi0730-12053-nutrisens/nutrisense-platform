namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

public record WeatherSnapshot(decimal TempC, string Condition, string WeatherType, DateTimeOffset At);

public interface IWeatherService
{
    Task<WeatherSnapshot> GetCurrentAsync(int cityId, CancellationToken ct = default);
}
