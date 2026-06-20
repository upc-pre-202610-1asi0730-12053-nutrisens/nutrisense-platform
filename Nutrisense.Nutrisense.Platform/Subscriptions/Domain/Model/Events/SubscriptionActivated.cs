using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Events;

public record SubscriptionActivated(int UserSubscriptionId, int UserId) : DomainEventBase;
