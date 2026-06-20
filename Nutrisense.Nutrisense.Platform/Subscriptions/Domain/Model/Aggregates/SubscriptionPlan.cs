namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

public class SubscriptionPlan
{
    public int Id { get; private set; }
    public string Key { get; private set; } = null!;
    public decimal PriceMonthly { get; private set; }
    public decimal? PriceAnnual { get; private set; }
    public List<string> Features { get; private set; } = new();
    public string Currency { get; private set; } = "USD";

    protected SubscriptionPlan() { }

    public SubscriptionPlan(string key, decimal priceMonthly, List<string> features,
        string currency = "USD", decimal? priceAnnual = null)
    {
        Key = key;
        PriceMonthly = priceMonthly;
        PriceAnnual = priceAnnual;
        Features = features;
        Currency = currency;
    }

    /// <summary>Returns the effective charge amount for the given billing period.</summary>
    public decimal GetEffectivePrice(string billingPeriod) =>
        billingPeriod == ValueObjects.BillingPeriod.Annual && PriceAnnual.HasValue
            ? PriceAnnual.Value
            : PriceMonthly;
}
