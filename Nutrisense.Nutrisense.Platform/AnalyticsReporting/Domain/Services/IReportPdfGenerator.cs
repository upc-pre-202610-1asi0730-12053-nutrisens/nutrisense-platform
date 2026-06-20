using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Services;

/// <summary>Domain service that renders a user's report for a date range into PDF bytes.</summary>
public interface IReportPdfGenerator
{
    Task<byte[]> GenerateAsync(int userId, DateRange range, CancellationToken ct = default);
}
