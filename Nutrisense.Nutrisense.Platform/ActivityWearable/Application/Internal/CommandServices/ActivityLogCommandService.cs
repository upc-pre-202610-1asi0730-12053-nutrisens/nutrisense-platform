using Cortex.Mediator;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Internal.CommandServices;

/// <summary>Implementation of <see cref="IActivityLogCommandService"/>. Persists activity logs and publishes the activity/caloric-balance event chain.</summary>
public class ActivityLogCommandService(
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    ICaloricBalanceCalculator caloricBalanceCalculator,
    ILogger<ActivityLogCommandService> logger,
    IMediator mediator) : IActivityLogCommandService
{
    /// <summary>Validates and persists a manual activity, then publishes the import, active-calories and balance-adjusted events.</summary>
    /// <param name="command">The command carrying the activity details.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The created <see cref="ActivityLog"/> on success, or a <see cref="LogManualActivityError"/> describing the failure.</returns>
    public async Task<Result<ActivityLog, LogManualActivityError>> Handle(LogManualActivityCommand command, CancellationToken ct = default)
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
                return new Result<ActivityLog, LogManualActivityError>.Failure(LogManualActivityError.InvalidActivity);
            }

            await activityLogRepository.AddAsync(log, ct);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new ActivityImported(command.UserId, command.Date, 1));

            var logs = await activityLogRepository.FindByUserAndDateAsync(command.UserId, command.Date, ct);
            var activeCalories = caloricBalanceCalculator.CalculateDailyActiveCalories(logs, command.Date);
            await mediator.PublishAsync(new ActiveCaloriesCalculated(command.UserId, command.Date, activeCalories));

            var balance = caloricBalanceCalculator.CalculateBalance(0, activeCalories, 0);
            await mediator.PublishAsync(new CaloricBalanceAdjusted(command.UserId, command.Date, balance));

            return new Result<ActivityLog, LogManualActivityError>.Success(log);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging manual activity for user {UserId}", command.UserId);
            return new Result<ActivityLog, LogManualActivityError>.Failure(LogManualActivityError.UnexpectedError);
        }
    }

    /// <summary>Removes an activity log after verifying ownership, then recalculates and publishes that day's caloric balance.</summary>
    /// <param name="command">The command identifying the activity log and requesting user.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>True on success, or a <see cref="DeleteActivityLogError"/> describing the failure.</returns>
    public async Task<Result<bool, DeleteActivityLogError>> Handle(DeleteActivityLogCommand command, CancellationToken ct = default)
    {
        try
        {
            var log = await activityLogRepository.FindByIdAsync(command.ActivityLogId, ct);
            if (log is null)
                return new Result<bool, DeleteActivityLogError>.Failure(DeleteActivityLogError.NotFound);

            if (log.UserId != command.UserId)
                return new Result<bool, DeleteActivityLogError>.Failure(DeleteActivityLogError.NotOwner);

            var date = log.Date;
            var userId = log.UserId;

            activityLogRepository.Remove(log);
            await unitOfWork.CompleteAsync(ct);

            var remaining = await activityLogRepository.FindByUserAndDateAsync(userId, date, ct);
            var activeCalories = caloricBalanceCalculator.CalculateDailyActiveCalories(remaining, date);
            var balance = caloricBalanceCalculator.CalculateBalance(0, activeCalories, 0);
            await mediator.PublishAsync(new CaloricBalanceAdjusted(userId, date, balance));

            return new Result<bool, DeleteActivityLogError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting activity log {ActivityLogId}", command.ActivityLogId);
            return new Result<bool, DeleteActivityLogError>.Failure(DeleteActivityLogError.UnexpectedError);
        }
    }
}
