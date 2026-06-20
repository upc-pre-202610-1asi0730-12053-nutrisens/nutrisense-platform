using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Events;

public record PlanSelected(int UserSubscriptionId, int UserId, string PlanKey) : DomainEventBase;
