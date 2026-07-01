using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Extensions;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Resources;
using Swashbuckle.AspNetCore.Annotations;
using SharedProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST;

[ApiController]
[Route("api/v1/activity-logs")]
[Authorize]
[Tags("Activity Logs")]
[Produces("application/json")]
[Consumes("application/json")]
/// <summary>REST endpoints for logging, querying and deleting user activity logs.</summary>
public class ActivityLogsController(
    IActivityLogCommandService commandService,
    IActivityLogQueryService queryService,
    IStringLocalizer<ActivityWearableMessages> localizer) : ControllerBase
{
    /// <summary>Logs a manual activity and triggers the caloric balance recalculation chain.</summary>
    /// <param name="resource">The activity payload to log.</param>
    /// <returns>201 Created with the logged activity, or 400 Bad Request on invalid input.</returns>
    [HttpPost]
    [SwaggerOperation("Log a manual activity and trigger caloric balance chain (Subflow 5.2)")]
    [SwaggerResponse(StatusCodes.Status201Created, "Activity logged successfully. Returns the created activity log.", typeof(ActivityLogResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The date is not in the expected yyyy-MM-dd format or the activity data is invalid.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected server error occurred while logging the activity.", typeof(ProblemDetails))]
    public async Task<IActionResult> LogManualActivity([FromBody] LogManualActivityResource resource)
    {
        if (resource.UserId != this.GetAuthenticatedUserId()) return Forbid();
        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd", out _))
            return BadRequest(SharedProblemDetailsFactory.Create(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidDateFormat"].Value, HttpContext.Request.Path));

        var command = ActivityLogAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return ActivityWearableActionResultAssembler.ToActionResult(result, localizer,
            log => new ObjectResult(ActivityLogAssembler.ToResource(log)) { StatusCode = StatusCodes.Status201Created },
            HttpContext.Request.Path);
    }

    /// <summary>Returns a user's activity logs, optionally filtered by a date range.</summary>
    /// <param name="userId">Identifier of the user whose logs are requested.</param>
    /// <param name="from">Optional inclusive lower bound (yyyy-MM-dd).</param>
    /// <param name="to">Optional inclusive upper bound (yyyy-MM-dd).</param>
    /// <returns>200 OK with the activity resources, or 400 Bad Request on an invalid date.</returns>
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation("Get activity logs for a user with optional date range")]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity logs retrieved successfully.", typeof(ActivityLogResource[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "One of the provided dates is not in the expected yyyy-MM-dd format.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetByUser(
        int userId,
        [FromQuery] string? from = null,
        [FromQuery] string? to = null)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        DateOnly? fromDate = null;
        DateOnly? toDate = null;

        if (from is not null && !DateOnly.TryParseExact(from, "yyyy-MM-dd", out var parsedFrom))
            return BadRequest(SharedProblemDetailsFactory.Create(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidFromDateFormat"].Value, HttpContext.Request.Path));
        else if (from is not null)
            fromDate = DateOnly.ParseExact(from, "yyyy-MM-dd", null);

        if (to is not null && !DateOnly.TryParseExact(to, "yyyy-MM-dd", out var parsedTo))
            return BadRequest(SharedProblemDetailsFactory.Create(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidToDateFormat"].Value, HttpContext.Request.Path));
        else if (to is not null)
            toDate = DateOnly.ParseExact(to, "yyyy-MM-dd", null);

        var logs = await queryService.Handle(new GetActivityLogsByUserQuery(userId, fromDate, toDate));
        return Ok(logs.Select(ActivityLogAssembler.ToResource));
    }

    /// <summary>Returns the aggregated daily activity summary for a user on a specific date.</summary>
    /// <param name="userId">Identifier of the user whose summary is requested.</param>
    /// <param name="date">Day to summarize (yyyy-MM-dd).</param>
    /// <returns>200 OK with the summary resource, or 400 Bad Request on an invalid date.</returns>
    [HttpGet("by-user/{userId:int}/daily-summary")]
    [SwaggerOperation("Get the daily activity summary for a user on a specific date")]
    [SwaggerResponse(StatusCodes.Status200OK, "Daily activity summary retrieved successfully.", typeof(DailyActivitySummaryResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided date is not in the expected yyyy-MM-dd format.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetDailySummary(int userId, [FromQuery] string date)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(SharedProblemDetailsFactory.Create(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidDateFormat"].Value, HttpContext.Request.Path));

        var summary = await queryService.Handle(new GetDailyActivitySummaryQuery(userId, parsedDate));
        return Ok(ActivityLogAssembler.ToSummaryResource(summary));
    }

    /// <summary>Deletes an activity log entry on behalf of its owner and recalculates that day's caloric balance.</summary>
    /// <param name="id">Identifier of the activity log to delete.</param>
    /// <param name="userId">Identifier of the requesting user, used to enforce ownership.</param>
    /// <returns>204 No Content on success, 403 Forbidden if not the owner, or 404 Not Found if it does not exist.</returns>
    [HttpDelete("{id:int}")]
    [SwaggerOperation("Delete an activity log entry and recalculate the caloric balance for that day")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Activity log deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "You are not allowed to delete this activity log entry.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested activity log entry does not exist.", typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteActivityLog(int id, [FromQuery] int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new DeleteActivityLogCommand(id, userId));
        return ActivityWearableActionResultAssembler.ToActionResult(result, localizer,
            _ => NoContent(), HttpContext.Request.Path);
    }
}
