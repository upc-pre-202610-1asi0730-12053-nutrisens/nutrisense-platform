namespace Nutrisense.Nutrisense.Platform.Shared.Domain.Services;

public interface ISubscriptionTierLookup
{
    Task<string> GetPlanKeyAsync(int userId, CancellationToken ct = default);
    Task<bool> IsProOrAboveAsync(int userId, CancellationToken ct = default);
    Task<bool> IsPremiumAsync(int userId, CancellationToken ct = default);
}
