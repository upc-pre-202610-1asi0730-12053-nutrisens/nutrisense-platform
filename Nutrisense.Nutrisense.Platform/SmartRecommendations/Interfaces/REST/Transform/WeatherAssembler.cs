using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;

public static class WeatherAssembler
{
    public static WeatherResource ToResource(int cityId, WeatherSnapshot snapshot) =>
        new(cityId, snapshot.TempC, snapshot.Condition, snapshot.WeatherType, snapshot.At);
}
