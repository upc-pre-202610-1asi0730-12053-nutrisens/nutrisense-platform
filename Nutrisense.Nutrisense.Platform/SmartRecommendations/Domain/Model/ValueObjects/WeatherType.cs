namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.ValueObjects;

public sealed record WeatherType(string Value)
{
    private static readonly string[] Allowed = ["hot", "warm", "mild", "cold"];

    public string Value { get; } = Allowed.Contains(Value)
        ? Value
        : throw new ArgumentException($"Invalid weather type: {Value}");
}
