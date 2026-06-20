namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

/// <summary>
/// Request payload to select and activate a subscription plan for a user.
/// </summary>
public record SelectSubscriptionPlanResource(
    /// <summary>Identifier of the user subscribing to the plan.</summary>
    int UserId,
    /// <summary>Unique string key of the subscription plan (e.g., basic, premium, enterprise).</summary>
    string PlanKey,
    /// <summary>Identifier of the payment method to use for billing.</summary>
    int PaymentMethodId,
    /// <summary>Billing period frequency. Valid values: monthly, annual. Default: monthly.</summary>
    string BillingPeriod = Domain.Model.ValueObjects.BillingPeriod.Monthly);
