namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal;

/// <summary>Read model carrying a user's progress snapshots across a date range for charting.</summary>
public record ProgressChartData(
    int UserId,
    DateOnly From,
    DateOnly To,
    IEnumerable<ProgressSnapshotDto> Snapshots);

/// <summary>Single day's calories and adherence point within a progress chart read model.</summary>
public record ProgressSnapshotDto(DateOnly Date, decimal TotalCalories, decimal AdherenceScore);
