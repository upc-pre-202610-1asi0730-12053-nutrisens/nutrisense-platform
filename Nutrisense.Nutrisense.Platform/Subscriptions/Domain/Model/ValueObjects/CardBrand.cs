namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;

public sealed record CardBrand
{
    private static readonly string[] AllowedBrands = ["visa", "mastercard", "amex"];

    public string Value { get; }

    public CardBrand(string value)
    {
        if (!AllowedBrands.Contains(value))
            throw new ArgumentException($"Invalid card brand '{value}'. Must be one of: {string.Join(", ", AllowedBrands)}.");
        Value = value;
    }
}
