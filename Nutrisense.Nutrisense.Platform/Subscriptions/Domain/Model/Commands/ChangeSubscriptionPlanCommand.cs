namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;

public record ChangeSubscriptionPlanCommand(
    int UserSubscriptionId,
    string NewPlanKey,
    string BillingPeriod = ValueObjects.BillingPeriod.Monthly,
    int? PaymentMethodId = null);
