using Nutrisense.Nutrisense.Platform.Shared.Application.Internal.EventHandlers;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.EventHandlers;

public class OnBenefitsEnabledHandler(
    IRecsEngineCommandService recsEngine,
    ILogger<OnBenefitsEnabledHandler> logger) : IEventHandler<BenefitsEnabled>
{
    public async Task Handle(BenefitsEnabled notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "BenefitsEnabled for user {UserId} with plan {PlanKey}. Unlocking premium features.",
            notification.UserId, notification.PlanKey);
        await recsEngine.Handle(
            new UnlockPremiumFeaturesCommand(notification.UserId, notification.PlanKey),
            cancellationToken);
    }
}
