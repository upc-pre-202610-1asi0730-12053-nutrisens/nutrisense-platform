using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Errors;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/analytics")]
[Authorize]
[Tags("Analytics")]
[Produces("application/json")]
/// <summary>REST endpoints serving a user's analytics dashboards, charts, streaks, and report exports.</summary>
public class AnalyticsController(
    IAnalyticsCommandService commandService,
    IAnalyticsQueryService queryService,
    IStringLocalizer<AnalyticsReportingMessages> localizer) : ControllerBase
{
    [HttpGet("dashboard/by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get dashboard data for a user", Description = "Retrieves dashboard metrics for a specific date including calories, macros, adherence and streak. Also records the dashboard view and updates user's streak.")]
    [SwaggerResponse(200, "Dashboard data retrieved successfully", typeof(DashboardResource))]
    [SwaggerResponse(400, "The provided date is not in the expected yyyy-MM-dd format.", typeof(ErrorResponse))]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetDashboard(int userId, [FromQuery] string date)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(new ErrorResponse(localizer["InvalidDateFormat"].Value));

        await commandService.Handle(new ViewDashboardCommand(userId));

        var data = await queryService.Handle(new GetDashboardQuery(userId, parsedDate));

        return Ok(DashboardResourceAssembler.ToResourceFromData(data));
    }

    [HttpGet("progress/by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get progress chart data", Description = "Retrieves daily progress snapshots including calories and adherence scores within the specified date range.")]
    [SwaggerResponse(200, "Progress chart data retrieved successfully", typeof(ProgressChartResource))]
    [SwaggerResponse(400, "One of the provided dates is not in the expected yyyy-MM-dd format.", typeof(ErrorResponse))]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetProgressChart(
        int userId, [FromQuery] string from, [FromQuery] string to)
    {
        if (!DateOnly.TryParseExact(from, "yyyy-MM-dd", out var fromDate)
            || !DateOnly.TryParseExact(to, "yyyy-MM-dd", out var toDate))
            return BadRequest(new ErrorResponse(localizer["InvalidDateFormat"].Value));

        var data = await queryService.Handle(new GetProgressChartQuery(userId, fromDate, toDate));

        return Ok(ProgressChartResourceAssembler.ToResourceFromData(data));
    }

    [HttpGet("streaks/by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get streak data", Description = "Retrieves the user's current and longest streaks along with weekly completion rate and list of recently completed dates.")]
    [SwaggerResponse(200, "Streak data retrieved successfully", typeof(StreakResource))]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetStreak(int userId)
    {
        var data = await queryService.Handle(new GetStreakQuery(userId));

        return Ok(StreakResourceAssembler.ToResourceFromData(data));
    }

    [HttpPost("dashboard-views/{userId:int}")]
    [SwaggerOperation(Summary = "Record dashboard view", Description = "Records that the user has viewed the dashboard, which updates the user's streak counter.")]
    [SwaggerResponse(204, "Dashboard view recorded successfully")]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    [SwaggerResponse(500, "An unexpected server error occurred while recording the dashboard view.", typeof(ErrorResponse))]
    public async Task<IActionResult> RecordDashboardView(int userId)
    {
        var result = await commandService.Handle(new ViewDashboardCommand(userId));
        return result switch
        {
            Result<bool, ViewDashboardError>.Success => NoContent(),
            Result<bool, ViewDashboardError>.Failure f => StatusCode(500,
                new ErrorResponse(localizer["UnexpectedErrorProcessingRequest"].Value)),
            _ => StatusCode(500)
        };
    }

    [HttpPost("export/{userId:int}")]
    [SwaggerOperation(Summary = "Export PDF report", Description = "Exports a PDF report containing analytics data for the specified date range. Requires a Premium subscription.")]
    [SwaggerResponse(200, "PDF report exported successfully")]
    [SwaggerResponse(400, "The provided date is not in the expected yyyy-MM-dd format or the date range is not valid.", typeof(ErrorResponse))]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    [SwaggerResponse(403, "This feature requires an active premium subscription.", typeof(ErrorResponse))]
    [SwaggerResponse(500, "An unexpected server error occurred while exporting the report.", typeof(ErrorResponse))]
    public async Task<IActionResult> ExportReport(
        int userId, [FromQuery] string from, [FromQuery] string to)
    {
        if (!DateOnly.TryParseExact(from, "yyyy-MM-dd", out var fromDate)
            || !DateOnly.TryParseExact(to, "yyyy-MM-dd", out var toDate))
            return BadRequest(new ErrorResponse(localizer["InvalidDateFormat"].Value));

        var result = await commandService.Handle(new ExportReportPdfCommand(userId, fromDate, toDate));

        return result switch
        {
            Result<(byte[] Pdf, string FileName), ExportReportPdfError>.Success s =>
                File(s.Value.Pdf, "application/pdf", s.Value.FileName),
            Result<(byte[] Pdf, string FileName), ExportReportPdfError>.Failure f =>
                f.Error switch
                {
                    ExportReportPdfError.PremiumRequired =>
                        StatusCode(403, new ErrorResponse(localizer["PremiumRequired"].Value)),
                    ExportReportPdfError.InvalidDateRange =>
                        BadRequest(new ErrorResponse(localizer["InvalidDateRange"].Value)),
                    _ => StatusCode(500, new ErrorResponse(localizer["UnexpectedErrorProcessingRequest"].Value))
                },
            _ => StatusCode(500)
        };
    }
}
