namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;

/// <param name="Date">
/// The day to evaluate, in the user's local calendar (carried by the originating
/// log event). When null, falls back to UTC "today" — used by triggers not tied to
/// a specific logged day (e.g. TDEE recalculation).
/// </param>
/// <summary>Request to compute adherence, snapshots, and streak insights for a user's day.</summary>
public record GenerateProgressInsightsCommand(int UserId, DateOnly? Date = null);
