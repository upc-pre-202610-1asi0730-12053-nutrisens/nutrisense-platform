namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

/// <summary>
/// Represents an active subscription for a user.
/// </summary>
public record UserSubscriptionResource(
    /// <summary>Unique identifier for the user subscription.</summary>
    int Id,
    /// <summary>Identifier of the user who owns this subscription.</summary>
    int UserId,
    /// <summary>Identifier of the selected subscription plan.</summary>
    int PlanId,
    /// <summary>String key of the subscription plan (e.g., basic, premium).</summary>
    string PlanKey,
    /// <summary>Current subscription status. Valid values: active, cancelled, expired, suspended.</summary>
    string Status,
    /// <summary>Billing period frequency. Valid values: monthly, annual.</summary>
    string BillingPeriod,
    /// <summary>Start date of the current billing period. Format: ISO 8601 (yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    DateTimeOffset PeriodStart,
    /// <summary>End date of the current billing period. Format: ISO 8601 (yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    DateTimeOffset PeriodEnd,
    /// <summary>Flag indicating whether the subscription is scheduled to cancel at the end of the current period.</summary>
    bool CancelAtPeriodEnd,
    /// <summary>Stripe subscription identifier used to manage the subscription in Stripe's system.</summary>
    string? StripeSubscriptionId,
    /// <summary>UTC timestamp of the last plan change. Null if plan has never been changed. Format: ISO 8601 (yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    DateTimeOffset? LastPlanChangeAt,
    /// <summary>Identifier of the payment method currently used for recurring charges. Null if no payment method is set.</summary>
    int? PaymentMethodId);
