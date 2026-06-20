using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.Errors;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;

public interface IPaymentMethodCommandService
{
    Task<Result<PaymentMethod, RegisterPaymentMethodError>> Handle(RegisterPaymentMethodCommand command);
    Task<bool> RemoveAsync(int id, CancellationToken ct = default);
}
