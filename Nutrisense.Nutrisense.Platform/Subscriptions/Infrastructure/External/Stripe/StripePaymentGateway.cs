using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.External.Stripe;

public class StripePaymentGateway : IPaymentGateway
{
    // TODO: replace with real Stripe SDK call
    public Task<PaymentResult> ChargeAsync(
        string stripeCustomerId,
        string stripePaymentMethodId,
        Money amount,
        CancellationToken ct)
        => Task.FromResult(new PaymentResult(true, Guid.NewGuid().ToString(), null));
}
