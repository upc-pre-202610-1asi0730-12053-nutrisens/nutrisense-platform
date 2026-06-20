using Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.Internal.QueryServices;

public class SubscriptionPlanQueryService(ISubscriptionPlanRepository planRepository)
    : ISubscriptionPlanQueryService
{
    public async Task<IEnumerable<SubscriptionPlan>> Handle(GetAllSubscriptionPlansQuery query) =>
        await planRepository.ListAsync();

    public Task<SubscriptionPlan?> Handle(GetSubscriptionPlanByKeyQuery query) =>
        planRepository.FindByKeyAsync(query.Key);
}
