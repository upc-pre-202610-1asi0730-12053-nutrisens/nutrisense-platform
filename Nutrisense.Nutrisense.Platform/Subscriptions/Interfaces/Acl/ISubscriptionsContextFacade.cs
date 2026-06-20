namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.Acl;

/// <summary>
/// Public Anti-Corruption-Layer contract for the Subscriptions bounded context.
/// Other BCs consume subscription-tier data through this facade without coupling to
/// Subscriptions' domain model: every parameter and return value is a primitive — never a
/// Command, aggregate or entity. Every method degrades gracefully, returning a neutral value
/// (the "basic" tier / false) on failure instead of throwing.
/// </summary>
public interface ISubscriptionsContextFacade
{
    /// <summary>
    /// Returns the plan key of the user's active subscription (e.g. "basic", "pro", "premium"),
    /// or "basic" when the user has no active subscription / on failure.
    /// </summary>
    Task<string> GetPlanKey(int userId, CancellationToken ct = default);

    /// <summary>
    /// Returns true when the user's active plan is "pro" or "premium", false otherwise / on failure.
    /// </summary>
    Task<bool> IsProOrAbove(int userId, CancellationToken ct = default);

    /// <summary>
    /// Returns true when the user's active plan is "premium", false otherwise / on failure.
    /// </summary>
    Task<bool> IsPremium(int userId, CancellationToken ct = default);
}
