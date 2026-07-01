using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;

/// <summary>Application service handling analytics commands such as insights, dashboard views, and exports.</summary>
public interface IAnalyticsCommandService
{
    Task<Result<bool, AnalyticsReportingError>> Handle(
        GenerateProgressInsightsCommand command, CancellationToken ct = default);

    Task<Result<bool, AnalyticsReportingError>> Handle(
        ViewDashboardCommand command, CancellationToken ct = default);
}
