using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Events;

public record PlanChanged(
    int UserSubscriptionId,
    int UserId,
    string OldPlanKey,
    string NewPlanKey) : DomainEventBase;
