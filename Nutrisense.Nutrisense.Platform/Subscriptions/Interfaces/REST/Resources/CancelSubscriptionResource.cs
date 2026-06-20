namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

/// <summary>
/// Request payload to cancel a subscription.
/// </summary>
public record CancelSubscriptionResource(
    /// <summary>Flag to determine cancellation timing. If true, subscription cancels at end of current period; if false, cancels immediately.</summary>
    bool CancelAtPeriodEnd);
