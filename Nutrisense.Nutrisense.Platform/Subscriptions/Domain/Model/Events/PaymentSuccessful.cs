using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Events;

public record PaymentSuccessful(int UserSubscriptionId, int UserId, decimal Amount) : DomainEventBase;
