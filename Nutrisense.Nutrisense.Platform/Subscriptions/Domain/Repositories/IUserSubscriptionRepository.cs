using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

public interface IUserSubscriptionRepository : IBaseRepository<UserSubscription>
{
    Task<UserSubscription?> FindByUserIdAsync(int userId, CancellationToken ct = default);
    Task<UserSubscription?> FindActiveByUserIdAsync(int userId, CancellationToken ct = default);
}
