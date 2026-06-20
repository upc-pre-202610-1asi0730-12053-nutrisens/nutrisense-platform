using Microsoft.Extensions.DependencyInjection;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.Shared.Application.Internal.EventHandlers;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal.EventHandlers;

/// <summary>Regenerates analytics insights when a user's meal consumption changes.</summary>
public class OnConsumptionUpdatedAnalyticsHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<OnConsumptionUpdatedAnalyticsHandler> logger) : IEventHandler<ConsumptionUpdated>
{
    public async Task Handle(ConsumptionUpdated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "ConsumptionUpdated received for user {UserId}. Triggering analytics insights generation.",
            notification.UserId);
        // Run on an isolated DI scope (its own DbContext): this notification has multiple
        // handlers that the mediator invokes concurrently, so sharing the request-scoped
        // DbContext would raise "A second operation was started on this context instance".
        await using var scope = scopeFactory.CreateAsyncScope();
        var svc = scope.ServiceProvider.GetRequiredService<IAnalyticsCommandService>();
        await svc.Handle(
            new GenerateProgressInsightsCommand(notification.UserId, notification.Date), cancellationToken);
    }
}
