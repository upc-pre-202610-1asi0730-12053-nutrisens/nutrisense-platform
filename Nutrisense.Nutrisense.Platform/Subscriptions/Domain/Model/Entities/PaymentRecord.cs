namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Entities;

public class PaymentRecord
{
    public int Id { get; set; }
    public int UserSubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTimeOffset ProcessedAt { get; set; }
    public string? StripePaymentIntentId { get; set; }

    protected PaymentRecord() { }

    public PaymentRecord(int userSubscriptionId, decimal amount, string currency, string status, string? stripePaymentIntentId)
    {
        UserSubscriptionId = userSubscriptionId;
        Amount = amount;
        Currency = currency;
        Status = status;
        ProcessedAt = DateTimeOffset.UtcNow;
        StripePaymentIntentId = stripePaymentIntentId;
    }
}
