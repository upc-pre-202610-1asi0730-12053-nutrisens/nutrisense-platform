using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Errors;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;

/// <summary>Application service handling analytics commands such as insights, dashboard views, and exports.</summary>
public interface IAnalyticsCommandService
{
    Task<Result<bool, GenerateProgressInsightsError>> Handle(
        GenerateProgressInsightsCommand command, CancellationToken ct = default);

    Task<Result<bool, ViewDashboardError>> Handle(
        ViewDashboardCommand command, CancellationToken ct = default);

    Task<Result<(byte[] Pdf, string FileName), ExportReportPdfError>> Handle(
        ExportReportPdfCommand command, CancellationToken ct = default);
}
