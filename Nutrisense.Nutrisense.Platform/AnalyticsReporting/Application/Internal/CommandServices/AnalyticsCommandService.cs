using Cortex.Mediator;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Services;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal.CommandServices;

/// <summary>Coordinates analytics write operations: insight generation, dashboard views, and PDF export.</summary>
public class AnalyticsCommandService(
    IUserAnalyticsRepository userAnalyticsRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    INutritionTrackingContextFacade nutritionTrackingFacade,
    IBodyHealthMetricsContextFacade bodyHealthMetricsFacade,
    IActivityWearableContextFacade activityWearableFacade,
    IAdherenceCalculator adherenceCalculator,
    ILogger<AnalyticsCommandService> logger) : IAnalyticsCommandService
{
    public async Task<Result<bool, AnalyticsReportingError>> Handle(
        GenerateProgressInsightsCommand command, CancellationToken ct = default)
    {
        try
        {
            var analytics = await userAnalyticsRepository.FindOrCreateAsync(command.UserId, ct);
            // Evaluate the day the log belongs to (user-local), not UTC "today" — otherwise an
            // evening log for a user behind UTC would be scored against an empty next-UTC-day.
            var today = command.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var actual = await nutritionTrackingFacade.GetDailyMacroSummary(command.UserId, today, ct);
            var goal = await bodyHealthMetricsFacade.GetActiveGoalByUserId(command.UserId, ct);

            if (goal is null)
                logger.LogInformation(
                    "User {UserId} has no active goal; the streak and adherence cannot advance for {Date} until a goal is set.",
                    command.UserId, today);

            var adherenceScore = adherenceCalculator.Calculate(actual, goal);
            analytics.UpdateAdherence(adherenceScore);

            _ = await activityWearableFacade.GetDailyCaloriesBurned(command.UserId, today, ct);

            var mealTypes = await nutritionTrackingFacade.GetLoggedMealTypes(command.UserId, today, ct);
            var isComplete = IsStreakDayMet(actual, goal, mealTypes);

            var snapshot = new ProgressSnapshot(
                today,
                actual?.TotalCalories ?? 0,
                actual?.TotalProteinG ?? 0,
                actual?.TotalCarbsG ?? 0,
                actual?.TotalFatG ?? 0,
                adherenceScore,
                isComplete);

            analytics.AddProgressSnapshot(snapshot);

            if (isComplete)
                analytics.UpdateStreak(today);

            // Weekly completion counts only fully-complete days (same criterion as the streak),
            // taking the latest snapshot per day so re-logging reflects the current state.
            var sevenDaysAgo = today.AddDays(-6);
            var completeDays = analytics.ProgressSnapshots
                .Where(s => s.Date >= sevenDaysAgo && s.Date <= today)
                .GroupBy(s => s.Date)
                .Count(g => g.OrderByDescending(s => s.CreatedAt).First().IsComplete);
            analytics.UpdateWeeklyCompletionRate(completeDays / 7.0m);

            if (analytics.Id != 0)
                userAnalyticsRepository.Update(analytics);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new UserDataAggregated(command.UserId, today));
            await mediator.PublishAsync(new ProgressCalculated(command.UserId, adherenceScore));
            await mediator.PublishAsync(new VisualizationGenerated(command.UserId));

            return new Result<bool, AnalyticsReportingError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error generating progress insights for user {UserId}", command.UserId);
            return new Result<bool, AnalyticsReportingError>.Failure(AnalyticsReportingError.UnexpectedError);
        }
    }

    public async Task<Result<bool, AnalyticsReportingError>> Handle(
        ViewDashboardCommand command, CancellationToken ct = default)
    {
        try
        {
            await mediator.PublishAsync(new DashboardViewed(command.UserId));
            return new Result<bool, AnalyticsReportingError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error viewing dashboard for user {UserId}", command.UserId);
            return new Result<bool, AnalyticsReportingError>.Failure(AnalyticsReportingError.UnexpectedError);
        }
    }

    private static readonly string[] RequiredMeals = ["breakfast", "lunch", "dinner"];

    private static bool IsStreakDayMet(
        DailyMacroSummaryItem? actual, UserGoalSummaryItem? goal, IReadOnlyList<string> mealTypes)
    {
        if (actual is null || goal is null) return false;
        if (goal.DailyCalorieTarget <= 0) return false;

        var calPct = (double)actual.TotalCalories / goal.DailyCalorieTarget * 100;
        if (calPct < 80 || calPct > 105) return false;

        if (!RequiredMeals.All(mealTypes.Contains)) return false;

        var effectiveProteinGoal = goal.ProteinTargetG > 0 ? goal.ProteinTargetG : 50m;
        return actual.TotalProteinG / effectiveProteinGoal * 100 >= 50;
    }

}
