namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;

public sealed record SubscriptionStatus
{
    private static readonly string[] AllowedStatuses = ["pending-payment", "active", "cancelled", "past-due"];

    public string Value { get; }

    public SubscriptionStatus(string value)
    {
        if (!AllowedStatuses.Contains(value))
            throw new ArgumentException($"Invalid subscription status '{value}'.");
        Value = value;
    }
}
