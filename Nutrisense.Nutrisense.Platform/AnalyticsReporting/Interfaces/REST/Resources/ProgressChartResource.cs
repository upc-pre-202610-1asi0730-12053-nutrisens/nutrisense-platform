namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Resources;

/// <summary>API response exposing a user's progress snapshots over a date range.</summary>
public record ProgressChartResource(
    /// <summary>The user's unique identifier.</summary>
    int UserId,
    /// <summary>The start date of the range in ISO 8601 format (yyyy-MM-dd).</summary>
    string From,
    /// <summary>The end date of the range in ISO 8601 format (yyyy-MM-dd).</summary>
    string To,
    /// <summary>Collection of daily progress snapshots within the specified date range.</summary>
    IEnumerable<ProgressSnapshotResource> Snapshots);

/// <summary>API response item for a single day's calories and adherence within a progress chart.</summary>
public record ProgressSnapshotResource(
    /// <summary>The date of the snapshot in ISO 8601 format (yyyy-MM-dd).</summary>
    string Date,
    /// <summary>Total calories consumed on this date in kilocalories (kcal).</summary>
    decimal TotalCalories,
    /// <summary>The user's adherence score for this date as a percentage (0-100).</summary>
    decimal AdherenceScore);
