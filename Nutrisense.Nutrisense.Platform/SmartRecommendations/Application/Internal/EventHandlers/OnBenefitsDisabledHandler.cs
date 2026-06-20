using Nutrisense.Nutrisense.Platform.Shared.Application.Internal.EventHandlers;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.EventHandlers;

public class OnBenefitsDisabledHandler(
    IRecsEngineCommandService recsEngine,
    ILogger<OnBenefitsDisabledHandler> logger) : IEventHandler<BenefitsDisabled>
{
    public async Task Handle(BenefitsDisabled notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "BenefitsDisabled for user {UserId}. Locking premium features.",
            notification.UserId);
        await recsEngine.Handle(
            new LockPremiumFeaturesCommand(notification.UserId),
            cancellationToken);
    }
}
