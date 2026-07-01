using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.Internal.CommandServices;

public class PaymentMethodCommandService(
    IPaymentMethodRepository paymentMethodRepository,
    IUnitOfWork unitOfWork,
    ILogger<PaymentMethodCommandService> logger) : IPaymentMethodCommandService
{
    public async Task<Result<PaymentMethod, SubscriptionsError>> Handle(RegisterPaymentMethodCommand command)
    {
        try
        {
            try
            {
                _ = new CardBrand(command.Brand);
                _ = new CardExpiry(command.ExpiryMonth, command.ExpiryYear);
            }
            catch (ArgumentException)
            {
                return new Result<PaymentMethod, SubscriptionsError>.Failure(
                    SubscriptionsError.InvalidCard);
            }

            var paymentMethod = new PaymentMethod(command);
            await paymentMethodRepository.AddAsync(paymentMethod);
            await unitOfWork.CompleteAsync();

            return new Result<PaymentMethod, SubscriptionsError>.Success(paymentMethod);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering payment method for user {UserId}", command.UserId);
            return new Result<PaymentMethod, SubscriptionsError>.Failure(
                SubscriptionsError.UnexpectedError);
        }
    }

    public async Task<bool> RemoveAsync(int id, CancellationToken ct = default)
    {
        var paymentMethod = await paymentMethodRepository.FindByIdAsync(id, ct);
        if (paymentMethod is null) return false;
        paymentMethodRepository.Remove(paymentMethod);
        await unitOfWork.CompleteAsync(ct);
        return true;
    }
}
