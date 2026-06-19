namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record ActivityLevel
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "sedentary", "lightly-active", "moderately-active", "very-active"
    };

    public string Value { get; }

    public ActivityLevel(string value)
    {
        if (!Allowed.Contains(value))
            throw new ArgumentException(
                $"'{value}' is not a valid activity level. Allowed: sedentary, lightly-active, moderately-active, very-active.",
                nameof(value));
        Value = value.ToLowerInvariant();
    }
}
