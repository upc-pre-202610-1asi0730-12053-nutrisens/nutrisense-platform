using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Transform;

public static class PaymentRecordResourceAssembler
{
    public static PaymentRecordResource ToResource(PaymentRecord r) =>
        new(r.Id, r.UserSubscriptionId, r.Amount, r.Currency, r.Status,
            r.ProcessedAt, r.StripePaymentIntentId);
}
