using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Errors;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;

public interface IPaymentMethodCommandService
{
    Task<Result<PaymentMethod, SubscriptionsError>> Handle(RegisterPaymentMethodCommand command);
    Task<bool> RemoveAsync(int id, CancellationToken ct = default);
}
