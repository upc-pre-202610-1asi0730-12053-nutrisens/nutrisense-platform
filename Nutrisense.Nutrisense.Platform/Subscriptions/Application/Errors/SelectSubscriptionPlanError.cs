namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.Errors;

public enum SelectSubscriptionPlanError
{
    PlanNotFound,
    PaymentMethodNotFound,
    PaymentFailed,
    AlreadySubscribed,
    UnexpectedError
}
