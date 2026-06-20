using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

public interface ISubscriptionPlanRepository : IBaseRepository<SubscriptionPlan>
{
    Task<SubscriptionPlan?> FindByKeyAsync(string key, CancellationToken ct = default);
}
