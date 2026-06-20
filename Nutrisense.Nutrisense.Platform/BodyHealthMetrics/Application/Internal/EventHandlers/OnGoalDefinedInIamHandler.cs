using Cortex.Mediator.Notifications;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Internal.EventHandlers;

/// <summary>Reacts to a goal-defined event from IAM by eagerly computing daily caloric targets if body metrics are ready.</summary>
public class OnGoalDefinedInIamHandler(
    IBodyMetricsCommandService commandService,
    ILogger<OnGoalDefinedInIamHandler> logger) : INotificationHandler<GoalDefined>
{
    public async Task Handle(GoalDefined notification, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new CalculateDailyCaloricGoalCommand(notification.UserId.Value));
        if (result.IsFailure)
            logger.LogInformation(
                "Skipping daily caloric goal recalculation for user {UserId}: body metrics not ready.",
                notification.UserId.Value);
    }
}
