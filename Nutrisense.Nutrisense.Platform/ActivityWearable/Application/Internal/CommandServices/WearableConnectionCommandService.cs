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

/// <summary>Implementation of <see cref="IWearableConnectionCommandService"/>. Manages the connect/sync/disconnect lifecycle and orchestrates activity imports from the provider.</summary>
public class WearableConnectionCommandService(
    IWearableConnectionRepository wearableConnectionRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IWearableSyncProvider syncProvider,
    ICaloricBalanceCalculator caloricBalanceCalculator,
    ILogger<WearableConnectionCommandService> logger,
    IMediator mediator) : IWearableConnectionCommandService
{
    /// <summary>Authorizes with the provider, creates the connection, imports the initial activity batch and publishes the device-connected and activity events.</summary>
    /// <param name="command">The command carrying the user, provider and OAuth code.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The created <see cref="WearableConnection"/> on success, or a <see cref="ConnectDeviceError"/> describing the failure.</returns>
    public async Task<Result<WearableConnection, ConnectDeviceError>> Handle(ConnectDeviceCommand command, CancellationToken ct = default)
    {
        try
        {
            var existing = await wearableConnectionRepository.FindByUserAndProviderAsync(command.UserId, command.Provider, ct);
            if (existing is not null && existing.Status == "connected")
                return new Result<WearableConnection, ConnectDeviceError>.Failure(ConnectDeviceError.AlreadyConnected);

            WearableConnection connection;
            try
            {
                connection = new WearableConnection(command, DateTimeOffset.UtcNow);
            }
            catch (ArgumentException)
            {
                return new Result<WearableConnection, ConnectDeviceError>.Failure(ConnectDeviceError.InvalidProvider);
            }

            var auth = await syncProvider.AuthorizeAsync(command.OAuthCode, ct);
            if (!auth.Success)
                return new Result<WearableConnection, ConnectDeviceError>.Failure(ConnectDeviceError.AuthorizationFailed);

            connection.ApplySync(DateTimeOffset.UtcNow);
            await wearableConnectionRepository.AddAsync(connection, ct);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new DeviceConnected(command.UserId, connection.Id, connection.Provider));

            await RunSubflow51(connection, command.UserId, ct);

            return new Result<WearableConnection, ConnectDeviceError>.Success(connection);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error connecting device for user {UserId}", command.UserId);
            return new Result<WearableConnection, ConnectDeviceError>.Failure(ConnectDeviceError.UnexpectedError);
        }
    }

    /// <summary>Fetches the last week of activities from the provider, persists the non-duplicate entries, stamps the sync and publishes the activity events.</summary>
    /// <param name="command">The command identifying the connection to sync.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The updated <see cref="WearableConnection"/> on success, or a <see cref="SyncActivityDataError"/> describing the failure.</returns>
    public async Task<Result<WearableConnection, SyncActivityDataError>> Handle(SyncActivityDataCommand command, CancellationToken ct = default)
    {
        try
        {
            var connection = await wearableConnectionRepository.FindByIdAsync(command.WearableConnectionId, ct);
            if (connection is null)
                return new Result<WearableConnection, SyncActivityDataError>.Failure(SyncActivityDataError.ConnectionNotFound);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = today.AddDays(-7);
            var imported = await syncProvider.FetchActivitiesAsync(connection.Id, from, today, ct);

            var existingLogs = await activityLogRepository.FindByUserAndDateAsync(connection.UserId, today, ct);
            var existingKeys = existingLogs
                .Select(l => (l.ActivityType, l.DurationMinutes, l.Date))
                .ToHashSet();

            var newLogs = new List<ActivityLog>();
            foreach (var activity in imported)
            {
                if (existingKeys.Contains((activity.ActivityType, activity.DurationMinutes, activity.Date)))
                    continue;

                var log = new ActivityLog(connection.UserId, activity.Date, activity.ActivityType,
                    activity.DurationMinutes, activity.Intensity, activity.CaloriesBurned, "google-fit");
                await activityLogRepository.AddAsync(log, ct);
                newLogs.Add(log);
            }

            connection.ApplySync(DateTimeOffset.UtcNow);
            wearableConnectionRepository.Update(connection);
            await unitOfWork.CompleteAsync(ct);

            await PublishActivityChain(connection.UserId, today, newLogs.Count, ct);

            return new Result<WearableConnection, SyncActivityDataError>.Success(connection);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error syncing activity data for connection {ConnectionId}", command.WearableConnectionId);
            return new Result<WearableConnection, SyncActivityDataError>.Failure(SyncActivityDataError.UnexpectedError);
        }
    }

    /// <summary>Transitions an existing connection to the disconnected state and persists the change.</summary>
    /// <param name="command">The command identifying the connection to disconnect.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>True on success, or a <see cref="DisconnectDeviceError"/> describing the failure.</returns>
    public async Task<Result<bool, DisconnectDeviceError>> Handle(DisconnectDeviceCommand command, CancellationToken ct = default)
    {
        try
        {
            var connection = await wearableConnectionRepository.FindByIdAsync(command.WearableConnectionId, ct);
            if (connection is null)
                return new Result<bool, DisconnectDeviceError>.Failure(DisconnectDeviceError.ConnectionNotFound);

            connection.ApplyDisconnect();
            wearableConnectionRepository.Update(connection);
            await unitOfWork.CompleteAsync(ct);

            return new Result<bool, DisconnectDeviceError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error disconnecting device {ConnectionId}", command.WearableConnectionId);
            return new Result<bool, DisconnectDeviceError>.Failure(DisconnectDeviceError.UnexpectedError);
        }
    }

    private async Task RunSubflow51(WearableConnection connection, int userId, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var imported = await syncProvider.FetchActivitiesAsync(connection.Id, today, today, ct);

        var newLogs = new List<ActivityLog>();
        foreach (var activity in imported)
        {
            var log = new ActivityLog(userId, activity.Date, activity.ActivityType,
                activity.DurationMinutes, activity.Intensity, activity.CaloriesBurned, "google-fit");
            await activityLogRepository.AddAsync(log, ct);
            newLogs.Add(log);
        }

        await unitOfWork.CompleteAsync(ct);

        await PublishActivityChain(userId, today, newLogs.Count, ct);
    }

    private async Task PublishActivityChain(int userId, DateOnly date, int count, CancellationToken ct)
    {
        await mediator.PublishAsync(new ActivityImported(userId, date, count));

        var logs = await activityLogRepository.FindByUserAndDateAsync(userId, date, ct);
        var activeCalories = caloricBalanceCalculator.CalculateDailyActiveCalories(logs, date);
        await mediator.PublishAsync(new ActiveCaloriesCalculated(userId, date, activeCalories));

        var balance = caloricBalanceCalculator.CalculateBalance(0, activeCalories, 0);
        await mediator.PublishAsync(new CaloricBalanceAdjusted(userId, date, balance));
    }
}
