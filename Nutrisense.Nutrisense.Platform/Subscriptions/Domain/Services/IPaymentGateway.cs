using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Services;

public record PaymentResult(bool Success, string? StripePaymentIntentId, string? FailureReason);

public interface IPaymentGateway
{
    /// <summary>
    /// Ensures a Stripe Customer exists for the user and that the given payment method is attached to it
    /// and set as the customer's default. When <paramref name="existingCustomerId"/> is provided it is
    /// reused; otherwise a new customer is created. Returns the Stripe customer id, or null on failure.
    /// </summary>
    Task<string?> EnsureCustomerAsync(
        int userId,
        string? existingCustomerId,
        string stripePaymentMethodId,
        CancellationToken ct);

    /// <summary>
    /// Charges the given amount to the customer's payment method by creating and confirming a
    /// PaymentIntent (off-session). Returns success with the PaymentIntent id, or a failure reason.
    /// </summary>
    Task<PaymentResult> ChargeAsync(
        string stripeCustomerId,
        string stripePaymentMethodId,
        Money amount,
        CancellationToken ct);
}
