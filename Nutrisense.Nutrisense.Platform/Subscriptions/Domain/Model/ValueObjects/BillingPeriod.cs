namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;

public sealed record BillingPeriod
{
    public const string Monthly = "monthly";
    public const string Annual  = "annual";

    private static readonly HashSet<string> Allowed = [Monthly, Annual];

    public string Value { get; }

    public BillingPeriod(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Allowed.Contains(value))
            throw new ArgumentException($"Billing period must be one of: {string.Join(", ", Allowed)}.");
        Value = value;
    }
}
