using Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.Internal.QueryServices;

public class UserSubscriptionQueryService(IUserSubscriptionRepository subscriptionRepository)
    : IUserSubscriptionQueryService
{
    public Task<UserSubscription?> Handle(GetUserSubscriptionByUserIdQuery query) =>
        subscriptionRepository.FindByUserIdAsync(query.UserId);

    public async Task<IEnumerable<PaymentRecord>> Handle(GetPaymentHistoryBySubscriptionIdQuery query)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(query.SubscriptionId);
        return subscription?.PaymentRecords ?? Enumerable.Empty<PaymentRecord>();
    }
}
