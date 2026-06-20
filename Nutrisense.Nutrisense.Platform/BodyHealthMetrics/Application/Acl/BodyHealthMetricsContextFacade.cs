using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.Acl;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Acl;

/// <summary>ACL facade that exposes body-health data to other bounded contexts using only primitive DTOs.</summary>
/// <inheritdoc cref="IBodyHealthMetricsContextFacade"/>
public class BodyHealthMetricsContextFacade(
    IBodyMetricsCommandService commandService,
    IBodyMetricsQueryService queryService) : IBodyHealthMetricsContextFacade
{
    public async Task<UserGoalSummaryItem?> GetActiveGoalByUserId(int userId, CancellationToken ct = default)
    {
        try
        {
            var metrics = await queryService.Handle(new GetBodyMetricsByUserIdQuery(userId));
            var goal = metrics?.GetActiveGoal();
            if (goal is null) return null;

            return new UserGoalSummaryItem(
                goal.Goal.Value,
                goal.DailyCalorieTarget,
                goal.ProteinTargetG,
                goal.CarbsTargetG,
                goal.FatTargetG);
        }
        catch
        {
            return null;
        }
    }
}
