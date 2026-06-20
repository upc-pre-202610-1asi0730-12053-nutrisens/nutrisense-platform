using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Transform;

public static class PaymentMethodResourceAssembler
{
    public static PaymentMethodResource ToResource(PaymentMethod m) =>
        new(m.Id, m.UserId, m.LastFour, m.Brand, m.ExpiryMonth, m.ExpiryYear,
            m.CardholderName, m.CreatedAt);
}
