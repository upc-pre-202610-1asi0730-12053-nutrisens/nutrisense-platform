using Microsoft.Extensions.DependencyInjection;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.Shared.Application.Internal.EventHandlers;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal.EventHandlers;

/// <summary>Regenerates analytics insights when a user's TDEE is recalculated.</summary>
public class OnTdeeCalculatedAnalyticsHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<OnTdeeCalculatedAnalyticsHandler> logger) : IEventHandler<TdeeCalculated>
{
    public async Task Handle(TdeeCalculated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "TdeeCalculated received for user {UserId}. Triggering analytics insights generation.",
            notification.UserId);
        await using var scope = scopeFactory.CreateAsyncScope();
        var svc = scope.ServiceProvider.GetRequiredService<IAnalyticsCommandService>();
        await svc.Handle(new GenerateProgressInsightsCommand(notification.UserId), cancellationToken);
    }
}
