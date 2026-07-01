using Stripe;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.External.Stripe;

/// <summary>
/// <see cref="IPaymentGateway"/> backed by the real Stripe API (test/sandbox mode). Reads the secret
/// key from the <c>Stripe:SecretKey</c> configuration. Creates a Stripe Customer per user, attaches the
/// payment method, and charges via a confirmed off-session PaymentIntent. When the secret key is not
/// configured every call fails gracefully (returning null / an unsuccessful result) instead of charging.
/// </summary>
public class StripePaymentGateway(IConfiguration configuration, ILogger<StripePaymentGateway> logger)
    : IPaymentGateway
{
    /// <summary>Returns the configured secret key, or null when Stripe is not configured.</summary>
    private string? SecretKey
    {
        get
        {
            var key = configuration["Stripe:SecretKey"];
            return string.IsNullOrWhiteSpace(key) ? null : key;
        }
    }

    /// <inheritdoc />
    public async Task<string?> EnsureCustomerAsync(
        int userId, string? existingCustomerId, string stripePaymentMethodId, CancellationToken ct)
    {
        var secret = SecretKey;
        if (secret is null)
        {
            logger.LogWarning("Stripe:SecretKey is not configured; cannot create a customer for user {UserId}.", userId);
            return null;
        }

        try
        {
            StripeConfiguration.ApiKey = secret;
            var requestOptions = new RequestOptions();

            var customerService = new CustomerService();
            var paymentMethodService = new PaymentMethodService();

            // Reuse the existing customer when present, otherwise create one tagged with the app user id.
            var customerId = existingCustomerId;
            if (string.IsNullOrWhiteSpace(customerId))
            {
                var customer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Metadata = new Dictionary<string, string> { ["app_user_id"] = userId.ToString() }
                }, requestOptions, ct);
                customerId = customer.Id;
            }

            // Attach the payment method to the customer (idempotent: a re-attach of the same PM is fine),
            // then make it the customer's default for off-session charges.
            await paymentMethodService.AttachAsync(stripePaymentMethodId,
                new PaymentMethodAttachOptions { Customer = customerId }, requestOptions, ct);

            await customerService.UpdateAsync(customerId, new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = stripePaymentMethodId }
            }, requestOptions, ct);

            return customerId;
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe customer/payment-method setup failed for user {UserId}.", userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PaymentResult> ChargeAsync(
        string stripeCustomerId, string stripePaymentMethodId, Money amount, CancellationToken ct)
    {
        var secret = SecretKey;
        if (secret is null)
        {
            logger.LogWarning("Stripe:SecretKey is not configured; declining charge.");
            return new PaymentResult(false, null, "Stripe is not configured.");
        }

        try
        {
            StripeConfiguration.ApiKey = secret;
            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = ToMinorUnits(amount.Amount),
                Currency = amount.Currency.ToLowerInvariant(),
                Customer = stripeCustomerId,
                PaymentMethod = stripePaymentMethodId,
                Confirm = true,
                OffSession = true
            }, new RequestOptions(), ct);

            return intent.Status == "succeeded"
                ? new PaymentResult(true, intent.Id, null)
                : new PaymentResult(false, intent.Id, $"PaymentIntent status: {intent.Status}");
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Stripe charge failed: {Message}", ex.StripeError?.Message ?? ex.Message);
            return new PaymentResult(false, null, ex.StripeError?.Message ?? ex.Message);
        }
    }

    /// <summary>Converts a decimal major-unit amount (e.g. 9.99) to Stripe minor units (e.g. 999 cents).</summary>
    private static long ToMinorUnits(decimal amount) => (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
}
