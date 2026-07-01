namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Errors;

public enum SubscriptionsError
{
    SubscriptionNotFound,
    SubscriptionNotActive,
    PlanNotFound,
    SamePlan,
    PaymentFailed,
    InvalidCard,
    PaymentMethodNotFound,
    AlreadySubscribed,
    UnexpectedError
}
