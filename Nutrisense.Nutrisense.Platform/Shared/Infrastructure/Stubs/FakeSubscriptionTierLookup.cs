using Nutrisense.Nutrisense.Platform.Shared.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Stubs;

public class FakeSubscriptionTierLookup : ISubscriptionTierLookup
{
    public Task<string> GetPlanKeyAsync(int userId, CancellationToken ct = default)
        => Task.FromResult("premium");

    public Task<bool> IsProOrAboveAsync(int userId, CancellationToken ct = default)
        => Task.FromResult(true);

    public Task<bool> IsPremiumAsync(int userId, CancellationToken ct = default)
        => Task.FromResult(true);
}
