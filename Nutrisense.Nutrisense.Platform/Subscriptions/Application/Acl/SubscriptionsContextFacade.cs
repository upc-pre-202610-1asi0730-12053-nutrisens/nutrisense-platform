using Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.Acl;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Application.Acl;

/// <inheritdoc cref="ISubscriptionsContextFacade"/>
public class SubscriptionsContextFacade(
    IUserSubscriptionCommandService commandService,
    IUserSubscriptionQueryService queryService) : ISubscriptionsContextFacade
{
    private const string DefaultPlanKey = "basic";
    private static readonly HashSet<string> ProOrAboveTiers = ["pro", "premium"];

    public async Task<string> GetPlanKey(int userId, CancellationToken ct = default)
    {
        try
        {
            var subscription = await queryService.Handle(new GetUserSubscriptionByUserIdQuery(userId));
            return subscription?.PlanKey ?? DefaultPlanKey;
        }
        catch
        {
            return DefaultPlanKey;
        }
    }

    public async Task<bool> IsProOrAbove(int userId, CancellationToken ct = default)
    {
        var planKey = await GetPlanKey(userId, ct);
        return ProOrAboveTiers.Contains(planKey);
    }

    public async Task<bool> IsPremium(int userId, CancellationToken ct = default)
    {
        var planKey = await GetPlanKey(userId, ct);
        return planKey == "premium";
    }
}
