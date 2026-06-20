using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;

public interface ISubscriptionPlanQueryService
{
    Task<IEnumerable<SubscriptionPlan>> Handle(GetAllSubscriptionPlansQuery query);
    Task<SubscriptionPlan?> Handle(GetSubscriptionPlanByKeyQuery query);
}
