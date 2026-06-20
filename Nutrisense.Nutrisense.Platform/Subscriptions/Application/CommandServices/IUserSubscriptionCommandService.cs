using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.Errors;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;

public interface IUserSubscriptionCommandService
{
    Task<Result<UserSubscription, SelectSubscriptionPlanError>> HandleSelectPlan(SelectSubscriptionPlanCommand command);
    Task<Result<UserSubscription, CancelSubscriptionError>> HandleCancel(CancelSubscriptionCommand command);
    Task<Result<UserSubscription, RenewSubscriptionError>> HandleRenew(RenewSubscriptionCommand command);
    Task<Result<UserSubscription, ChangeSubscriptionPlanError>> HandleChangePlan(ChangeSubscriptionPlanCommand command);
}
