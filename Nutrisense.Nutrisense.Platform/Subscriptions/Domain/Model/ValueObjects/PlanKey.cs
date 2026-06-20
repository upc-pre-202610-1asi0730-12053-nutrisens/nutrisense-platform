namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;

public sealed record PlanKey
{
    private static readonly string[] AllowedKeys = ["basic", "pro", "premium"];

    public string Value { get; }

    public PlanKey(string value)
    {
        if (!AllowedKeys.Contains(value))
            throw new ArgumentException($"Invalid plan key '{value}'. Must be one of: {string.Join(", ", AllowedKeys)}.");
        Value = value;
    }
}
