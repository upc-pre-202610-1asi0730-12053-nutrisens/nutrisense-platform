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

/// <summary>Implementation of <see cref="IWearableConnectionCommandService"/>. Manages the connect/sync/disconnect lifecycle and orchestrates activity imports from the provider.</summary>
public class WearableConnectionCommandService(
    IWearableConnectionRepository wearableConnectionRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IWearableSyncProvider syncProvider,
    ICaloricBalanceCalculator caloricBalanceCalculator,
    IActiveCalorieEstimator calorieEstimator,
    IBodyHealthMetricsContextFacade bodyHealthMetricsFacade,
    INutritionTrackingContextFacade nutritionTrackingFacade,
    ILogger<WearableConnectionCommandService> logger,
    IMediator mediator) : IWearableConnectionCommandService
{
    /// <summary>Body weight (kg) assumed for calorie estimation when the user has no recorded weight.</summary>
    private const decimal DefaultWeightKg = 70m;

    /// <summary>Authorizes with the provider, creates the connection, imports the initial activity batch and publishes the device-connected and activity events.</summary>
    /// <param name="command">The command carrying the user, provider and OAuth code.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The created <see cref="WearableConnection"/> on success, or a <see cref="ActivityWearableError"/> describing the failure.</returns>
    public async Task<Result<WearableConnection, ActivityWearableError>> Handle(ConnectDeviceCommand command, CancellationToken ct = default)
    {
        try
        {
            var existing = await wearableConnectionRepository.FindByUserAndProviderAsync(command.UserId, command.Provider, ct);
            if (existing is not null && existing.Status == "connected")
                return new Result<WearableConnection, ActivityWearableError>.Failure(ActivityWearableError.AlreadyConnected);

            WearableConnection connection;
            try
            {
                connection = new WearableConnection(command, DateTimeOffset.UtcNow);
            }
            catch (ArgumentException)
            {
                return new Result<WearableConnection, ActivityWearableError>.Failure(ActivityWearableError.InvalidProvider);
            }

            var auth = await syncProvider.AuthorizeAsync(command.OAuthCode, ct);
            if (!auth.Success)
                return new Result<WearableConnection, ActivityWearableError>.Failure(ActivityWearableError.AuthorizationFailed);

            connection.ApplyAuthorization(auth.AccessToken, auth.RefreshToken, auth.ExpiresAt);
            connection.ApplySync(DateTimeOffset.UtcNow);
            await wearableConnectionRepository.AddAsync(connection, ct);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new DeviceConnected(command.UserId, connection.Id, connection.Provider));

            await RunSubflow51(connection, command.UserId, ct);

            return new Result<WearableConnection, ActivityWearableError>.Success(connection);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error connecting device for user {UserId}", command.UserId);
            return new Result<WearableConnection, ActivityWearableError>.Failure(ActivityWearableError.UnexpectedError);
        }
    }

    /// <summary>Fetches the last week of activities from the provider, persists the non-duplicate entries, stamps the sync and publishes the activity events.</summary>
    /// <param name="command">The command identifying the connection to sync.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The updated <see cref="WearableConnection"/> on success, or a <see cref="ActivityWearableError"/> describing the failure.</returns>
    public async Task<Result<WearableConnection, ActivityWearableError>> Handle(SyncActivityDataCommand command, CancellationToken ct = default)
    {
        try
        {
            var connection = await wearableConnectionRepository.FindByIdAsync(command.WearableConnectionId, ct);
            if (connection is null)
                return new Result<WearableConnection, ActivityWearableError>.Failure(ActivityWearableError.WearableConnectionNotFound);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = today.AddDays(-7);
            var accessToken = await EnsureFreshAccessTokenAsync(connection, ct);
            var imported = await syncProvider.FetchActivitiesAsync(accessToken, from, today, ct);

            var existingLogs = await activityLogRepository.FindByUserAndDateAsync(connection.UserId, today, ct);
            var existingKeys = existingLogs
                .Select(l => (l.ActivityType, l.DurationMinutes, l.Date))
                .ToHashSet();

            var weightKg = await ResolveWeightKgAsync(connection.UserId, ct);

            var newLogs = new List<ActivityLog>();
            foreach (var activity in imported)
            {
                if (existingKeys.Contains((activity.ActivityType, activity.DurationMinutes, activity.Date)))
                    continue;

                var calories = ResolveCalories(activity, weightKg);
                var log = new ActivityLog(connection.UserId, activity.Date, activity.ActivityType,
                    activity.DurationMinutes, activity.Intensity, calories, connection.Provider);
                await activityLogRepository.AddAsync(log, ct);
                newLogs.Add(log);
            }

            connection.ApplySync(DateTimeOffset.UtcNow);
            wearableConnectionRepository.Update(connection);
            await unitOfWork.CompleteAsync(ct);

            await PublishActivityChain(connection.UserId, today, newLogs.Count, ct);

            return new Result<WearableConnection, ActivityWearableError>.Success(connection);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error syncing activity data for connection {ConnectionId}", command.WearableConnectionId);
            return new Result<WearableConnection, ActivityWearableError>.Failure(ActivityWearableError.UnexpectedError);
        }
    }

    /// <summary>Transitions an existing connection to the disconnected state and persists the change.</summary>
    /// <param name="command">The command identifying the connection to disconnect.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>True on success, or a <see cref="ActivityWearableError"/> describing the failure.</returns>
    public async Task<Result<bool, ActivityWearableError>> Handle(DisconnectDeviceCommand command, CancellationToken ct = default)
    {
        try
        {
            var connection = await wearableConnectionRepository.FindByIdAsync(command.WearableConnectionId, ct);
            if (connection is null)
                return new Result<bool, ActivityWearableError>.Failure(ActivityWearableError.WearableConnectionNotFound);

            connection.ApplyDisconnect();
            wearableConnectionRepository.Update(connection);
            await unitOfWork.CompleteAsync(ct);

            return new Result<bool, ActivityWearableError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error disconnecting device {ConnectionId}", command.WearableConnectionId);
            return new Result<bool, ActivityWearableError>.Failure(ActivityWearableError.UnexpectedError);
        }
    }

    /// <summary>Enables or disables automatic syncing on an existing connection and persists the change.</summary>
    /// <param name="command">The command identifying the connection and the desired auto-sync state.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The updated <see cref="WearableConnection"/> on success, or a <see cref="ActivityWearableError"/> describing the failure.</returns>
    public async Task<Result<WearableConnection, ActivityWearableError>> Handle(SetAutoSyncCommand command, CancellationToken ct = default)
    {
        try
        {
            var connection = await wearableConnectionRepository.FindByIdAsync(command.WearableConnectionId, ct);
            if (connection is null)
                return new Result<WearableConnection, ActivityWearableError>.Failure(ActivityWearableError.WearableConnectionNotFound);

            connection.SetAutoSync(command.Enabled);
            wearableConnectionRepository.Update(connection);
            await unitOfWork.CompleteAsync(ct);

            return new Result<WearableConnection, ActivityWearableError>.Success(connection);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating auto-sync for connection {ConnectionId}", command.WearableConnectionId);
            return new Result<WearableConnection, ActivityWearableError>.Failure(ActivityWearableError.UnexpectedError);
        }
    }

    /// <summary>Returns a non-expired access token, refreshing it through the provider when possible.</summary>
    private async Task<string> EnsureFreshAccessTokenAsync(WearableConnection connection, CancellationToken ct)
    {
        var stillValid = connection.TokenExpiresAt is { } exp && exp > DateTimeOffset.UtcNow.AddMinutes(1);
        if (stillValid || string.IsNullOrWhiteSpace(connection.RefreshToken))
            return connection.AccessToken ?? string.Empty;

        var refreshed = await syncProvider.RefreshAsync(connection.RefreshToken, ct);
        if (refreshed.Success)
            connection.ApplyAuthorization(refreshed.AccessToken, refreshed.RefreshToken, refreshed.ExpiresAt);

        return connection.AccessToken ?? string.Empty;
    }

    /// <summary>Resolves the user's current weight (kg) for calorie estimation, falling back to <see cref="DefaultWeightKg"/>.</summary>
    private async Task<decimal> ResolveWeightKgAsync(int userId, CancellationToken ct)
    {
        var weight = await bodyHealthMetricsFacade.GetCurrentWeightKg(userId, ct);
        if (weight is { } w && w > 0)
            return w;

        logger.LogInformation("No recorded weight for user {UserId}; using default {DefaultWeightKg}kg for calorie estimation.",
            userId, DefaultWeightKg);
        return DefaultWeightKg;
    }

    /// <summary>Returns the provider-reported calories, or a MET-based estimate when the provider reported none.</summary>
    private decimal ResolveCalories(ImportedActivity activity, decimal weightKg) =>
        activity.CaloriesBurned > 0
            ? activity.CaloriesBurned
            : calorieEstimator.Estimate(activity.ActivityType, activity.DurationMinutes, activity.Intensity, weightKg);

    private async Task RunSubflow51(WearableConnection connection, int userId, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var imported = await syncProvider.FetchActivitiesAsync(connection.AccessToken ?? string.Empty, today, today, ct);

        var weightKg = await ResolveWeightKgAsync(userId, ct);

        var newLogs = new List<ActivityLog>();
        foreach (var activity in imported)
        {
            var calories = ResolveCalories(activity, weightKg);
            var log = new ActivityLog(userId, activity.Date, activity.ActivityType,
                activity.DurationMinutes, activity.Intensity, calories, connection.Provider);
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

        var tdee = await bodyHealthMetricsFacade.GetTdee(userId, ct) ?? 0m;
        var consumed = (await nutritionTrackingFacade.GetDailyMacroSummary(userId, date, ct))?.TotalCalories ?? 0m;
        var balance = caloricBalanceCalculator.CalculateBalance(tdee, activeCalories, consumed);
        await mediator.PublishAsync(new CaloricBalanceAdjusted(userId, date, balance));
    }
}
