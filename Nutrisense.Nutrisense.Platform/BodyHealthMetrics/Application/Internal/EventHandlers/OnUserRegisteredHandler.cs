using Cortex.Mediator.Notifications;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Internal.EventHandlers;

/// <summary>Logs an informational message when a user registers, prompting body-metrics onboarding.</summary>
public class OnUserRegisteredHandler(ILogger<OnUserRegisteredHandler> logger) : INotificationHandler<UserRegistered>
{
    public Task Handle(UserRegistered notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "User {UserId} registered. Awaiting body metrics registration via POST /api/v1/body-metrics.",
            notification.UserId.Value);
        return Task.CompletedTask;
    }
}
