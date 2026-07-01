using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Errors;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;

public interface IUserSubscriptionCommandService
{
    Task<Result<UserSubscription, SubscriptionsError>> HandleSelectPlan(SelectSubscriptionPlanCommand command);
    Task<Result<UserSubscription, SubscriptionsError>> HandleCancel(CancelSubscriptionCommand command);
    Task<Result<UserSubscription, SubscriptionsError>> HandleRenew(RenewSubscriptionCommand command);
    Task<Result<UserSubscription, SubscriptionsError>> HandleChangePlan(ChangeSubscriptionPlanCommand command);
}
