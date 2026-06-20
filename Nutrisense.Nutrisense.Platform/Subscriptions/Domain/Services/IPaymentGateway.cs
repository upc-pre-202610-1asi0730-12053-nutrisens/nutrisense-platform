using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Services;

public record PaymentResult(bool Success, string? StripePaymentIntentId, string? FailureReason);

public interface IPaymentGateway
{
    Task<PaymentResult> ChargeAsync(
        string stripeCustomerId,
        string stripePaymentMethodId,
        Money amount,
        CancellationToken ct);
}
