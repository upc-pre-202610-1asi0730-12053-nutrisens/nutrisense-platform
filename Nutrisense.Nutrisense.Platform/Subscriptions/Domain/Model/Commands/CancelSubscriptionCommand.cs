namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;

public record CancelSubscriptionCommand(int UserSubscriptionId, bool CancelAtPeriodEnd);
