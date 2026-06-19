namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record PreferredUnits
{
    public string Value { get; }

    public PreferredUnits(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Preferred units cannot be empty.", nameof(value));
        Value = value.Trim().ToLowerInvariant();
    }
}
