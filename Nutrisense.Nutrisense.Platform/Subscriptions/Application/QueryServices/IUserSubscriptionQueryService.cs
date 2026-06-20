using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;

public interface IUserSubscriptionQueryService
{
    Task<UserSubscription?> Handle(GetUserSubscriptionByUserIdQuery query);
    Task<IEnumerable<PaymentRecord>> Handle(GetPaymentHistoryBySubscriptionIdQuery query);
}
