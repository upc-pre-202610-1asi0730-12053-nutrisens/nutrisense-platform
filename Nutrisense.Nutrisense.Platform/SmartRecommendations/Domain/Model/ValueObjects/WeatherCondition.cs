namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.ValueObjects;

public sealed record WeatherCondition(string Value)
{
    private static readonly string[] Allowed = ["sunny", "cloudy", "rainy", "partly-cloudy"];

    public string Value { get; } = Allowed.Contains(Value)
        ? Value
        : throw new ArgumentException($"Invalid weather condition: {Value}");
}
