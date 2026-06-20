namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.Errors;

public enum ChangeSubscriptionPlanError
{
    SubscriptionNotFound,
    PlanNotFound,
    SamePlan,
    PaymentFailed,
    UnexpectedError
}
