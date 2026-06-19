namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.Acl;

/// <summary>
/// A user's active health goal, flattened to primitives for cross-BC consumption.
/// </summary>
public record UserGoalSummaryItem(
    string GoalType,
    int DailyCalorieTarget,
    decimal ProteinTargetG,
    decimal CarbsTargetG,
    decimal FatTargetG);

/// <summary>
/// Public Anti-Corruption-Layer contract for the BodyHealthMetrics bounded context.
/// Other BCs consume body/goal data through this facade without coupling to BodyHealthMetrics'
/// domain model: every parameter and return value is a primitive or a primitive-only DTO defined
/// here — never a Command, aggregate or entity. Every method degrades gracefully, returning null
/// on failure instead of throwing.
/// </summary>
public interface IBodyHealthMetricsContextFacade
{
    /// <summary>
    /// Returns the user's currently active health goal flattened to primitives,
    /// or null if there is none / on failure.
    /// </summary>
    Task<UserGoalSummaryItem?> GetActiveGoalByUserId(int userId, CancellationToken ct = default);
}
