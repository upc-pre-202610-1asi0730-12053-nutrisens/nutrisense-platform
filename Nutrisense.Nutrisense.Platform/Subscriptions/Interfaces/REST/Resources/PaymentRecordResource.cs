namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

/// <summary>
/// Represents a payment transaction record for a subscription.
/// </summary>
public record PaymentRecordResource(
    /// <summary>Unique identifier for the payment record.</summary>
    int Id,
    /// <summary>Identifier of the subscription associated with this payment.</summary>
    int UserSubscriptionId,
    /// <summary>Payment amount in the specified currency. Decimal precision to 2 places.</summary>
    decimal Amount,
    /// <summary>ISO 4217 currency code (e.g., USD, EUR).</summary>
    string Currency,
    /// <summary>Payment status. Valid values: succeeded, failed, pending, refunded.</summary>
    string Status,
    /// <summary>UTC timestamp when the payment was processed. Format: ISO 8601 (yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    DateTimeOffset ProcessedAt,
    /// <summary>Stripe payment intent identifier used to track the transaction in Stripe's system.</summary>
    string? StripePaymentIntentId);
