namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

/// <summary>
/// Represents a subscription plan available for purchase.
/// </summary>
public record SubscriptionPlanResource(
    /// <summary>Unique identifier for the subscription plan.</summary>
    int Id,
    /// <summary>Unique string key identifier for the plan (e.g., basic, premium, enterprise).</summary>
    string Key,
    /// <summary>Monthly billing price. Decimal precision to 2 places.</summary>
    decimal PriceMonthly,
    /// <summary>Annual billing price if available, null if plan does not support annual billing.</summary>
    decimal? PriceAnnual,
    /// <summary>List of feature descriptions included in this plan.</summary>
    List<string> Features,
    /// <summary>ISO 4217 currency code for pricing (e.g., USD, EUR).</summary>
    string Currency);
