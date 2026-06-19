namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record BiologicalSex
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "male", "female", "prefer-not-to-say"
    };

    public string Value { get; }

    public BiologicalSex(string value)
    {
        if (!Allowed.Contains(value))
            throw new ArgumentException(
                $"'{value}' is not a valid biological sex. Allowed: male, female, prefer-not-to-say.",
                nameof(value));
        Value = value.ToLowerInvariant();
    }
}
