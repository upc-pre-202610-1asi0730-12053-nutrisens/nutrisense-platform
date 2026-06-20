using Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.Internal.QueryServices;

public class PaymentMethodQueryService(IPaymentMethodRepository paymentMethodRepository)
    : IPaymentMethodQueryService
{
    public Task<IEnumerable<PaymentMethod>> Handle(GetPaymentMethodsByUserIdQuery query) =>
        paymentMethodRepository.FindByUserIdAsync(query.UserId);
}
