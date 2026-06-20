namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

/// <summary>
/// Request payload to register a new payment method for a user.
/// </summary>
public record RegisterPaymentMethodResource(
    /// <summary>Identifier of the user registering the payment method.</summary>
    int UserId,
    /// <summary>Last four digits of the card for display purposes.</summary>
    string LastFour,
    /// <summary>Card issuer brand. Valid values: visa, mastercard, amex, discover.</summary>
    string Brand,
    /// <summary>Card expiration month (1-12).</summary>
    int ExpiryMonth,
    /// <summary>Card expiration year (four-digit format, e.g., 2026).</summary>
    int ExpiryYear,
    /// <summary>Name of the cardholder as it appears on the card.</summary>
    string CardholderName,
    /// <summary>Stripe payment method token created after validating card details with Stripe.</summary>
    string StripePaymentMethodId);
