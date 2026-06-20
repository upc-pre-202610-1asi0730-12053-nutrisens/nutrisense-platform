namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.Acl;

/// <summary>
/// Public Anti-Corruption-Layer contract for the ActivityWearable bounded context.
/// Other BCs consume activity data through this facade without coupling to ActivityWearable's
/// domain model: every parameter and return value is a primitive — never a Command, aggregate
/// or entity. Every method degrades gracefully, returning a neutral value (0) on failure
/// instead of throwing.
/// </summary>
public interface IActivityWearableContextFacade
{
    /// <summary>
    /// Returns the total calories burned by the user on the given date,
    /// or 0 if there is no activity / on failure.
    /// </summary>
    Task<decimal> GetDailyCaloriesBurned(int userId, DateOnly date, CancellationToken ct = default);
}
