using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/activity-logs")]
[Authorize]
[Tags("Activity Logs")]
[Produces("application/json")]
[Consumes("application/json")]
/// <summary>REST endpoints for logging, querying and deleting user activity logs.</summary>
public class ActivityLogsController(
    IActivityLogCommandService commandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Logs a manual activity and triggers the caloric balance recalculation chain.</summary>
    /// <param name="resource">The activity payload to log.</param>
    /// <returns>201 Created with the logged activity, or 400 Bad Request on invalid input.</returns>
    [HttpPost]
    [SwaggerOperation("Log a manual activity and trigger caloric balance chain (Subflow 5.2)")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogManualActivity([FromBody] LogManualActivityResource resource)
    {
        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd", out _))
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd." });

        var command = ActivityLogAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return LogManualActivityResultAssembler.ToActionResult(result, localizer);
    }

    /// <summary>Deletes an activity log entry on behalf of its owner and recalculates that day's caloric balance.</summary>
    /// <param name="id">Identifier of the activity log to delete.</param>
    /// <param name="userId">Identifier of the requesting user, used to enforce ownership.</param>
    /// <returns>204 No Content on success, 403 Forbidden if not the owner, or 404 Not Found if it does not exist.</returns>
    [HttpDelete("{id:int}")]
    [SwaggerOperation("Delete an activity log entry and recalculate the caloric balance for that day")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteActivityLog(int id, [FromQuery] int userId)
    {
        var result = await commandService.Handle(new DeleteActivityLogCommand(id, userId));
        return DeleteActivityLogResultAssembler.ToActionResult(result, localizer);
    }
}
