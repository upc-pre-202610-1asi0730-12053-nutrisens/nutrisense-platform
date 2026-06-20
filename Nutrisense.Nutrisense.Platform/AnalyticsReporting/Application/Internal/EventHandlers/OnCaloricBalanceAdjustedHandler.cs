using Microsoft.Extensions.DependencyInjection;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Internal.EventHandlers;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal.EventHandlers;

/// <summary>Regenerates analytics insights when a user's caloric balance is adjusted by activity data.</summary>
public class OnCaloricBalanceAdjustedHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<OnCaloricBalanceAdjustedHandler> logger) : IEventHandler<CaloricBalanceAdjusted>
{
    public async Task Handle(CaloricBalanceAdjusted notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "CaloricBalanceAdjusted received for user {UserId}. Triggering analytics insights generation.",
            notification.UserId);
        // Isolated DI scope (own DbContext): this notification is handled concurrently with
        // other handlers, so sharing the request-scoped DbContext would cause a concurrency error.
        await using var scope = scopeFactory.CreateAsyncScope();
        var svc = scope.ServiceProvider.GetRequiredService<IAnalyticsCommandService>();
        await svc.Handle(
            new GenerateProgressInsightsCommand(notification.UserId), cancellationToken);
    }
}
