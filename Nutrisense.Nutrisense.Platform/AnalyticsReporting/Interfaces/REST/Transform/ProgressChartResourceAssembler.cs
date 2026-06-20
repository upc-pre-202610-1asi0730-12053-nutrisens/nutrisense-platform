using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Transform;

/// <summary>Maps progress chart read models to their API resource representation.</summary>
public static class ProgressChartResourceAssembler
{
    public static ProgressChartResource ToResourceFromData(ProgressChartData data)
        => new(
            data.UserId,
            data.From.ToString("yyyy-MM-dd"),
            data.To.ToString("yyyy-MM-dd"),
            data.Snapshots.Select(s => new ProgressSnapshotResource(
                s.Date.ToString("yyyy-MM-dd"),
                s.TotalCalories,
                s.AdherenceScore)));
}
