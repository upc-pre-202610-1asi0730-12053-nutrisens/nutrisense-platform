namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;

public record SelectSubscriptionPlanCommand(
    int UserId,
    string PlanKey,
    int PaymentMethodId,
    string BillingPeriod = ValueObjects.BillingPeriod.Monthly);
