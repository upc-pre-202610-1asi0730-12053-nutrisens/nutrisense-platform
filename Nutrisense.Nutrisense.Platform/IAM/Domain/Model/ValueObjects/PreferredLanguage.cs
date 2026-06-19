namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record PreferredLanguage
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "es", "en"
    };

    public string Value { get; }

    public PreferredLanguage(string value)
    {
        if (!Allowed.Contains(value))
            throw new ArgumentException(
                $"'{value}' is not a valid preferred language. Allowed: es, en.",
                nameof(value));
        Value = value.ToLowerInvariant();
    }
}
