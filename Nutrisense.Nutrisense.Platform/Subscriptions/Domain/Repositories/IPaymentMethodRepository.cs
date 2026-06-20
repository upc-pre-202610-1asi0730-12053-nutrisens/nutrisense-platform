using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

public interface IPaymentMethodRepository : IBaseRepository<PaymentMethod>
{
    Task<IEnumerable<PaymentMethod>> FindByUserIdAsync(int userId, CancellationToken ct = default);
}
