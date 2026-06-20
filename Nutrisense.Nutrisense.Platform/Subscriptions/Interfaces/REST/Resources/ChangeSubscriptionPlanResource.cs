namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

/// <summary>
/// Request payload to change an active subscription to a different plan.
/// </summary>
public record ChangeSubscriptionPlanResource(
    /// <summary>Unique string key of the target subscription plan (e.g., basic, premium, enterprise).</summary>
    string NewPlanKey,
    /// <summary>Billing period frequency for the new plan. Valid values: monthly, annual. Default: monthly.</summary>
    string BillingPeriod = Domain.Model.ValueObjects.BillingPeriod.Monthly,
    /// <summary>Identifier of the payment method to use for the plan change. Null to use the current payment method.</summary>
    int? PaymentMethodId = null);
