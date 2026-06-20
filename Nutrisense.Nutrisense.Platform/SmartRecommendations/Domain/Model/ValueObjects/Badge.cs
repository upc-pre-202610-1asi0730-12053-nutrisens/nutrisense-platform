namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.ValueObjects;

public sealed record Badge(string Value)
{
    private static readonly string[] Allowed = ["high-protein", "light", "bulk"];

    public string Value { get; } = Allowed.Contains(Value)
        ? Value
        : throw new ArgumentException($"Invalid badge: {Value}");
}
