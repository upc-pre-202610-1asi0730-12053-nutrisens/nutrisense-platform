namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;

public record RegisterPaymentMethodCommand(
    int UserId,
    string LastFour,
    string Brand,
    int ExpiryMonth,
    int ExpiryYear,
    string CardholderName,
    string StripePaymentMethodId);
