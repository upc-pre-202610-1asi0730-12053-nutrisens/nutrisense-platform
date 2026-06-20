using Nutrisense.Nutrisense.Platform.Shared.Domain.Services;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.CrossContext;

/// <summary>Read-only cross-BC lookup — no writes, no transactions.</summary>
public class CrossContextSubscriptionTierLookup(IUserSubscriptionRepository subscriptionRepository)
    : ISubscriptionTierLookup
{
    private static readonly HashSet<string> ProOrAbove = ["pro", "premium"];

    public async Task<string> GetPlanKeyAsync(int userId, CancellationToken ct = default)
    {
        var subscription = await subscriptionRepository.FindActiveByUserIdAsync(userId, ct);
        return subscription?.PlanKey ?? "basic";
    }

    public async Task<bool> IsProOrAboveAsync(int userId, CancellationToken ct = default)
    {
        var planKey = await GetPlanKeyAsync(userId, ct);
        return ProOrAbove.Contains(planKey);
    }

    public async Task<bool> IsPremiumAsync(int userId, CancellationToken ct = default)
    {
        var planKey = await GetPlanKeyAsync(userId, ct);
        return planKey == "premium";
    }
}
