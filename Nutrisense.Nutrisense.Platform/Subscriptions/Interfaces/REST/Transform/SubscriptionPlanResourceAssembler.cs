using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Transform;

public static class SubscriptionPlanResourceAssembler
{
    public static SubscriptionPlanResource ToResource(SubscriptionPlan p) =>
        new(p.Id, p.Key, p.PriceMonthly, p.PriceAnnual, p.Features, p.Currency);
}
