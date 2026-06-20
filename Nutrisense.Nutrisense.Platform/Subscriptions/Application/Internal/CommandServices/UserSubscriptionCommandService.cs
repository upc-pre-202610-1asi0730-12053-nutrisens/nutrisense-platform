using Cortex.Mediator;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.Errors;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.Internal.CommandServices;

public class UserSubscriptionCommandService(
    IUserSubscriptionRepository subscriptionRepository,
    ISubscriptionPlanRepository planRepository,
    IPaymentMethodRepository paymentMethodRepository,
    IPaymentGateway paymentGateway,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<UserSubscriptionCommandService> logger) : IUserSubscriptionCommandService
{
    public async Task<Result<UserSubscription, SelectSubscriptionPlanError>> HandleSelectPlan(
        SelectSubscriptionPlanCommand command)
    {
        try
        {
            // 1. Validate plan key VO, then find plan
            try { _ = new PlanKey(command.PlanKey); }
            catch (ArgumentException)
            {
                return new Result<UserSubscription, SelectSubscriptionPlanError>.Failure(
                    SelectSubscriptionPlanError.PlanNotFound);
            }

            var plan = await planRepository.FindByKeyAsync(command.PlanKey);
            if (plan is null)
                return new Result<UserSubscription, SelectSubscriptionPlanError>.Failure(
                    SelectSubscriptionPlanError.PlanNotFound);

            // 2. Reject if already subscribed
            var existing = await subscriptionRepository.FindActiveByUserIdAsync(command.UserId);
            if (existing is not null)
                return new Result<UserSubscription, SelectSubscriptionPlanError>.Failure(
                    SelectSubscriptionPlanError.AlreadySubscribed);

            // 3. Validate payment method exists
            var paymentMethod = await paymentMethodRepository.FindByIdAsync(command.PaymentMethodId);
            if (paymentMethod is null)
                return new Result<UserSubscription, SelectSubscriptionPlanError>.Failure(
                    SelectSubscriptionPlanError.PaymentMethodNotFound);

            // 4. Create subscription (status = "pending-payment") and save
            var subscription = new UserSubscription(command, plan.Id);
            await subscriptionRepository.AddAsync(subscription);
            await unitOfWork.CompleteAsync();

            // 5. Publish PlanSelected
            await mediator.PublishAsync(new PlanSelected(subscription.Id, command.UserId, command.PlanKey));

            // 6. Charge via payment gateway
            var amount = new Money(plan.PriceMonthly, plan.Currency);
            var chargeResult = await paymentGateway.ChargeAsync(
                $"cus_user_{command.UserId}",
                paymentMethod.StripePaymentMethodId,
                amount,
                CancellationToken.None);

            if (!chargeResult.Success)
                return new Result<UserSubscription, SelectSubscriptionPlanError>.Failure(
                    SelectSubscriptionPlanError.PaymentFailed);

            // 7. Add PaymentRecord, set status "active", save
            var record = new PaymentRecord(
                subscription.Id,
                plan.PriceMonthly,
                plan.Currency,
                "succeeded",
                chargeResult.StripePaymentIntentId);

            subscription.ApplyPayment(record);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync();

            // 8. Publish downstream events — BenefitsEnabled is CRITICAL
            await mediator.PublishAsync(new PaymentSuccessful(subscription.Id, command.UserId, plan.PriceMonthly));
            await mediator.PublishAsync(new SubscriptionActivated(subscription.Id, command.UserId));
            await mediator.PublishAsync(new BenefitsEnabled(command.UserId, command.PlanKey));

            return new Result<UserSubscription, SelectSubscriptionPlanError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error selecting subscription plan for user {UserId}", command.UserId);
            return new Result<UserSubscription, SelectSubscriptionPlanError>.Failure(
                SelectSubscriptionPlanError.UnexpectedError);
        }
    }

    public async Task<Result<UserSubscription, CancelSubscriptionError>> HandleCancel(
        CancelSubscriptionCommand command)
    {
        try
        {
            var subscription = await subscriptionRepository.FindByIdAsync(command.UserSubscriptionId);
            if (subscription is null)
                return new Result<UserSubscription, CancelSubscriptionError>.Failure(
                    CancelSubscriptionError.NotFound);

            if (subscription.Status != "active")
                return new Result<UserSubscription, CancelSubscriptionError>.Failure(
                    CancelSubscriptionError.NotActive);

            subscription.ApplyCancel(command.CancelAtPeriodEnd);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync();

            // BenefitsDisabled is CRITICAL
            await mediator.PublishAsync(new SubscriptionCancelled(subscription.Id, subscription.UserId));
            await mediator.PublishAsync(new BenefitsDisabled(subscription.UserId));

            return new Result<UserSubscription, CancelSubscriptionError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cancelling subscription {SubscriptionId}", command.UserSubscriptionId);
            return new Result<UserSubscription, CancelSubscriptionError>.Failure(
                CancelSubscriptionError.UnexpectedError);
        }
    }

    public async Task<Result<UserSubscription, RenewSubscriptionError>> HandleRenew(
        RenewSubscriptionCommand command)
    {
        try
        {
            var subscription = await subscriptionRepository.FindByIdAsync(command.UserSubscriptionId);
            if (subscription is null)
                return new Result<UserSubscription, RenewSubscriptionError>.Failure(
                    RenewSubscriptionError.NotFound);

            var plan = await planRepository.FindByIdAsync(subscription.PlanId);
            var paymentMethod = subscription.PaymentMethodId.HasValue
                ? await paymentMethodRepository.FindByIdAsync(subscription.PaymentMethodId.Value)
                : null;

            var amount = new Money(plan!.PriceMonthly, plan.Currency);
            var chargeResult = await paymentGateway.ChargeAsync(
                $"cus_user_{subscription.UserId}",
                paymentMethod?.StripePaymentMethodId ?? "pm_fake",
                amount,
                CancellationToken.None);

            if (!chargeResult.Success)
                return new Result<UserSubscription, RenewSubscriptionError>.Failure(
                    RenewSubscriptionError.PaymentFailed);

            var record = new PaymentRecord(
                subscription.Id,
                plan.PriceMonthly,
                plan.Currency,
                "succeeded",
                chargeResult.StripePaymentIntentId);

            var newPeriodEnd = subscription.PeriodEnd.AddMonths(1);
            subscription.ApplyRenewal(record, newPeriodEnd);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync();

            // BenefitsEnabled is CRITICAL
            await mediator.PublishAsync(new SubscriptionRenewed(subscription.Id, subscription.UserId));
            await mediator.PublishAsync(new BenefitsEnabled(subscription.UserId, subscription.PlanKey));

            return new Result<UserSubscription, RenewSubscriptionError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error renewing subscription {SubscriptionId}", command.UserSubscriptionId);
            return new Result<UserSubscription, RenewSubscriptionError>.Failure(
                RenewSubscriptionError.UnexpectedError);
        }
    }

    public async Task<Result<UserSubscription, ChangeSubscriptionPlanError>> HandleChangePlan(
        ChangeSubscriptionPlanCommand command)
    {
        try
        {
            // 1. Validate BillingPeriod VO
            try { _ = new BillingPeriod(command.BillingPeriod); }
            catch (ArgumentException)
            {
                return new Result<UserSubscription, ChangeSubscriptionPlanError>.Failure(
                    ChangeSubscriptionPlanError.PlanNotFound);
            }

            // 2. Load subscription
            var subscription = await subscriptionRepository.FindByIdAsync(command.UserSubscriptionId);
            if (subscription is null)
                return new Result<UserSubscription, ChangeSubscriptionPlanError>.Failure(
                    ChangeSubscriptionPlanError.SubscriptionNotFound);

            // 3. Reject same-plan change
            if (subscription.PlanKey == command.NewPlanKey)
                return new Result<UserSubscription, ChangeSubscriptionPlanError>.Failure(
                    ChangeSubscriptionPlanError.SamePlan);

            // 4. Load current and target plans
            var currentPlan = await planRepository.FindByIdAsync(subscription.PlanId);
            var newPlan = await planRepository.FindByKeyAsync(command.NewPlanKey);

            if (newPlan is null)
                return new Result<UserSubscription, ChangeSubscriptionPlanError>.Failure(
                    ChangeSubscriptionPlanError.PlanNotFound);

            var oldPlanKey = subscription.PlanKey;
            var isUpgrade = newPlan.PriceMonthly > (currentPlan?.PriceMonthly ?? 0m);

            if (isUpgrade)
            {
                // 5a. Upgrade: charge full new plan price, reset period
                var effectivePrice = newPlan.GetEffectivePrice(command.BillingPeriod);
                var chargeAmount = new Money(effectivePrice, newPlan.Currency);

                var paymentMethodId = command.PaymentMethodId ?? subscription.PaymentMethodId;
                var paymentMethod = paymentMethodId.HasValue
                    ? await paymentMethodRepository.FindByIdAsync(paymentMethodId.Value)
                    : null;

                var chargeResult = await paymentGateway.ChargeAsync(
                    $"cus_user_{subscription.UserId}",
                    paymentMethod?.StripePaymentMethodId ?? "pm_fake",
                    chargeAmount,
                    CancellationToken.None);

                if (!chargeResult.Success)
                    return new Result<UserSubscription, ChangeSubscriptionPlanError>.Failure(
                        ChangeSubscriptionPlanError.PaymentFailed);

                var record = new PaymentRecord(
                    subscription.Id,
                    effectivePrice,
                    newPlan.Currency,
                    "succeeded",
                    chargeResult.StripePaymentIntentId);

                subscription.ApplyPlanChangeWithPayment(command.NewPlanKey, newPlan.Id, record, command.BillingPeriod);
                subscriptionRepository.Update(subscription);
                await unitOfWork.CompleteAsync();

                await mediator.PublishAsync(new PaymentSuccessful(subscription.Id, subscription.UserId, effectivePrice));
                await mediator.PublishAsync(new PlanChanged(subscription.Id, subscription.UserId, oldPlanKey, command.NewPlanKey));
                await mediator.PublishAsync(new BenefitsEnabled(subscription.UserId, command.NewPlanKey));
            }
            else
            {
                // 5b. Downgrade: no charge, PeriodEnd unchanged
                subscription.ApplyPlanChangeWithoutPayment(command.NewPlanKey, newPlan.Id, command.BillingPeriod);
                subscriptionRepository.Update(subscription);
                await unitOfWork.CompleteAsync();

                await mediator.PublishAsync(new PlanChanged(subscription.Id, subscription.UserId, oldPlanKey, command.NewPlanKey));

                // Publish benefits event based on whether destination plan has paid features
                if (newPlan.PriceMonthly > 0m)
                    await mediator.PublishAsync(new BenefitsEnabled(subscription.UserId, command.NewPlanKey));
                else
                    await mediator.PublishAsync(new BenefitsDisabled(subscription.UserId));
            }

            return new Result<UserSubscription, ChangeSubscriptionPlanError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error changing subscription plan for subscription {SubscriptionId}", command.UserSubscriptionId);
            return new Result<UserSubscription, ChangeSubscriptionPlanError>.Failure(
                ChangeSubscriptionPlanError.UnexpectedError);
        }
    }
}
