using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Events;

public record SubscriptionRenewed(int UserSubscriptionId, int UserId) : DomainEventBase;
