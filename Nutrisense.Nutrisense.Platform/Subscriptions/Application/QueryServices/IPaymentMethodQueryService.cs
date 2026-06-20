using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;

public interface IPaymentMethodQueryService
{
    Task<IEnumerable<PaymentMethod>> Handle(GetPaymentMethodsByUserIdQuery query);
}
