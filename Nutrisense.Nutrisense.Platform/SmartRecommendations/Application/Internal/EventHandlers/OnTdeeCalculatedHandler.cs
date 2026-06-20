using Microsoft.Extensions.DependencyInjection;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.Shared.Application.Internal.EventHandlers;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.EventHandlers;

public class OnTdeeCalculatedHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<OnTdeeCalculatedHandler> logger) : IEventHandler<TdeeCalculated>
{
    public async Task Handle(TdeeCalculated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "TdeeCalculated received for user {UserId}. Triggering recommendation generation.",
            notification.UserId);
        await using var scope = scopeFactory.CreateAsyncScope();
        var svc = scope.ServiceProvider.GetRequiredService<IRecsEngineCommandService>();
        await svc.Handle(
            new GenerateRecommendationCommand(notification.UserId, "tdee-calculated"),
            cancellationToken);
    }
}
