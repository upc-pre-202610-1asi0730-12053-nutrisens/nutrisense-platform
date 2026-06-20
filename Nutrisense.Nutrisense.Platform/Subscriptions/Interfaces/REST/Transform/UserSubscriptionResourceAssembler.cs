using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Transform;

public static class UserSubscriptionResourceAssembler
{
    public static UserSubscriptionResource ToResource(UserSubscription s) =>
        new(s.Id, s.UserId, s.PlanId, s.PlanKey, s.Status, s.BillingPeriod,
            s.PeriodStart, s.PeriodEnd, s.CancelAtPeriodEnd,
            s.StripeSubscriptionId, s.LastPlanChangeAt, s.PaymentMethodId);
}
