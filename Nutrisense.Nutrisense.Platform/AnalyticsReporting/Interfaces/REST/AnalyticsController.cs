using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Extensions;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Resources;
using Swashbuckle.AspNetCore.Annotations;
using SharedProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST;

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
    [SwaggerResponse(400, "The provided date is not in the expected yyyy-MM-dd format.", typeof(ProblemDetails))]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetDashboard(int userId, [FromQuery] string date)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(SharedProblemDetailsFactory.Create(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidDateFormat"].Value, HttpContext.Request.Path));

        await commandService.Handle(new ViewDashboardCommand(userId));

        var data = await queryService.Handle(new GetDashboardQuery(userId, parsedDate));

        return Ok(DashboardResourceAssembler.ToResourceFromData(data));
    }

    [HttpGet("progress/by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get progress chart data", Description = "Retrieves daily progress snapshots including calories and adherence scores within the specified date range.")]
    [SwaggerResponse(200, "Progress chart data retrieved successfully", typeof(ProgressChartResource))]
    [SwaggerResponse(400, "One of the provided dates is not in the expected yyyy-MM-dd format.", typeof(ProblemDetails))]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetProgressChart(
        int userId, [FromQuery] string from, [FromQuery] string to)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        if (!DateOnly.TryParseExact(from, "yyyy-MM-dd", out var fromDate)
            || !DateOnly.TryParseExact(to, "yyyy-MM-dd", out var toDate))
            return BadRequest(SharedProblemDetailsFactory.Create(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidDateFormat"].Value, HttpContext.Request.Path));

        var data = await queryService.Handle(new GetProgressChartQuery(userId, fromDate, toDate));

        return Ok(ProgressChartResourceAssembler.ToResourceFromData(data));
    }

    [HttpGet("streaks/by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get streak data", Description = "Retrieves the user's current and longest streaks along with weekly completion rate and list of recently completed dates.")]
    [SwaggerResponse(200, "Streak data retrieved successfully", typeof(StreakResource))]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetStreak(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var data = await queryService.Handle(new GetStreakQuery(userId));

        return Ok(StreakResourceAssembler.ToResourceFromData(data));
    }

    [HttpPost("dashboard-views/{userId:int}")]
    [SwaggerOperation(Summary = "Record dashboard view", Description = "Records that the user has viewed the dashboard, which updates the user's streak counter.")]
    [SwaggerResponse(204, "Dashboard view recorded successfully")]
    [SwaggerResponse(401, "Authentication is required to access this resource.")]
    [SwaggerResponse(500, "An unexpected server error occurred while recording the dashboard view.", typeof(ProblemDetails))]
    public async Task<IActionResult> RecordDashboardView(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new ViewDashboardCommand(userId));
        return result switch
        {
            Result<bool, AnalyticsReportingError>.Success => NoContent(),
            Result<bool, AnalyticsReportingError>.Failure f =>
                AnalyticsReportingActionResultAssembler.ToActionResult(f.Error, localizer, Request.Path),
            _ => StatusCode(500)
        };
    }
}
