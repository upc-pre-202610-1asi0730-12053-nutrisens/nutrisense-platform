using Cortex.Mediator;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Internal.CommandServices;

/// <summary>Implementation of <see cref="IActivityLogCommandService"/>. Persists activity logs and publishes the activity/caloric-balance event chain.</summary>
public class ActivityLogCommandService(
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    ICaloricBalanceCalculator caloricBalanceCalculator,
    IBodyHealthMetricsContextFacade bodyHealthMetricsFacade,
    INutritionTrackingContextFacade nutritionTrackingFacade,
    ILogger<ActivityLogCommandService> logger,
    IMediator mediator) : IActivityLogCommandService
{
    /// <summary>Computes the day's net caloric balance by combining TDEE, logged active calories and consumed calories.</summary>
    private async Task<decimal> ComputeBalanceAsync(int userId, DateOnly date, decimal activeCalories, CancellationToken ct)
    {
        var tdee = await bodyHealthMetricsFacade.GetTdee(userId, ct) ?? 0m;
        var consumed = (await nutritionTrackingFacade.GetDailyMacroSummary(userId, date, ct))?.TotalCalories ?? 0m;
        return caloricBalanceCalculator.CalculateBalance(tdee, activeCalories, consumed);
    }

    /// <summary>Validates and persists a manual activity, then publishes the import, active-calories and balance-adjusted events.</summary>
    /// <param name="command">The command carrying the activity details.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The created <see cref="ActivityLog"/> on success, or a <see cref="ActivityWearableError"/> describing the failure.</returns>
    public async Task<Result<ActivityLog, ActivityWearableError>> Handle(LogManualActivityCommand command, CancellationToken ct = default)
    {
        try
        {
            ActivityLog log;
            try
            {
                log = new ActivityLog(command);
            }
            catch (ArgumentException)
            {
                return new Result<ActivityLog, ActivityWearableError>.Failure(ActivityWearableError.InvalidActivity);
            }

            await activityLogRepository.AddAsync(log, ct);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new ActivityImported(command.UserId, command.Date, 1));

            var logs = await activityLogRepository.FindByUserAndDateAsync(command.UserId, command.Date, ct);
            var activeCalories = caloricBalanceCalculator.CalculateDailyActiveCalories(logs, command.Date);
            await mediator.PublishAsync(new ActiveCaloriesCalculated(command.UserId, command.Date, activeCalories));

            var balance = await ComputeBalanceAsync(command.UserId, command.Date, activeCalories, ct);
            await mediator.PublishAsync(new CaloricBalanceAdjusted(command.UserId, command.Date, balance));

            return new Result<ActivityLog, ActivityWearableError>.Success(log);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging manual activity for user {UserId}", command.UserId);
            return new Result<ActivityLog, ActivityWearableError>.Failure(ActivityWearableError.UnexpectedError);
        }
    }

    /// <summary>Removes an activity log after verifying ownership, then recalculates and publishes that day's caloric balance.</summary>
    /// <param name="command">The command identifying the activity log and requesting user.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>True on success, or a <see cref="ActivityWearableError"/> describing the failure.</returns>
    public async Task<Result<bool, ActivityWearableError>> Handle(DeleteActivityLogCommand command, CancellationToken ct = default)
    {
        try
        {
            var log = await activityLogRepository.FindByIdAsync(command.ActivityLogId, ct);
            if (log is null)
                return new Result<bool, ActivityWearableError>.Failure(ActivityWearableError.ActivityLogNotFound);

            if (log.UserId != command.UserId)
                return new Result<bool, ActivityWearableError>.Failure(ActivityWearableError.ActivityLogNotOwner);

            var date = log.Date;
            var userId = log.UserId;

            activityLogRepository.Remove(log);
            await unitOfWork.CompleteAsync(ct);

            var remaining = await activityLogRepository.FindByUserAndDateAsync(userId, date, ct);
            var activeCalories = caloricBalanceCalculator.CalculateDailyActiveCalories(remaining, date);
            var balance = await ComputeBalanceAsync(userId, date, activeCalories, ct);
            await mediator.PublishAsync(new CaloricBalanceAdjusted(userId, date, balance));

            return new Result<bool, ActivityWearableError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting activity log {ActivityLogId}", command.ActivityLogId);
            return new Result<bool, ActivityWearableError>.Failure(ActivityWearableError.UnexpectedError);
        }
    }
}
