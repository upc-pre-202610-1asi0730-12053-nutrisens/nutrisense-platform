using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;

/// <summary>Maps SetHealthGoalResource to SetHealthGoalCommand.</summary>
public static class SetHealthGoalCommandAssembler
{
    public static SetHealthGoalCommand ToCommand(int userId, SetHealthGoalResource resource) =>
        new(userId, resource.Goal, resource.TargetWeightKg, resource.WeeklyRateKg);
}
