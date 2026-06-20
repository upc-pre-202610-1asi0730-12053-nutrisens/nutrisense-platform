namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

/// <summary>
/// Represents a registered payment method for a user.
/// </summary>
public record PaymentMethodResource(
    /// <summary>Unique identifier for the payment method record.</summary>
    int Id,
    /// <summary>Identifier of the user who owns this payment method.</summary>
    int UserId,
    /// <summary>Last four digits of the card for display and identification purposes.</summary>
    string LastFour,
    /// <summary>Card issuer brand. Valid values: visa, mastercard, amex, discover.</summary>
    string Brand,
    /// <summary>Card expiration month (1-12).</summary>
    int ExpiryMonth,
    /// <summary>Card expiration year (four-digit format, e.g., 2026).</summary>
    int ExpiryYear,
    /// <summary>Name of the cardholder as it appears on the card.</summary>
    string CardholderName,
    /// <summary>UTC timestamp when the payment method was registered. Format: ISO 8601 (yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    DateTimeOffset? CreatedAt);
