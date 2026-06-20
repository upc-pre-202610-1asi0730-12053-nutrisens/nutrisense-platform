using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/nutrition-logs")]
[Tags("Nutrition Logs")]
[Authorize]
[Produces("application/json")]
public class NutritionLogsController(
    INutritionLogCommandService commandService,
    INutritionLogQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation("Get all nutrition log entries for a user on a specific date")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByUserAndDate(int userId, [FromQuery] string date)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd." });

        var logs = await queryService.Handle(new GetNutritionLogByUserAndDateQuery(userId, parsedDate));
        return Ok(logs.Select(NutritionLogResourceAssembler.ToResource));
    }

    [HttpPatch("{entryId:int}")]
    [Consumes("application/json")]
    [SwaggerOperation("Update the quantity of an existing nutrition log entry")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEntry(int entryId, [FromBody] UpdateNutritionLogResource resource)
    {
        var command = UpdateNutritionLogEntryCommandAssembler.ToCommand(entryId, resource);
        var result = await commandService.Handle(command);
        return UpdateNutritionLogEntryResultAssembler.ToActionResult(result, localizer, Request.Path);
    }

    [HttpDelete("{entryId:int}")]
    [SwaggerOperation("Delete a nutrition log entry")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEntry(int entryId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue("sub");

        if (!int.TryParse(userIdClaim, out var authenticatedUserId))
            return Unauthorized();

        var command = new DeleteNutritionLogEntryCommand(entryId, authenticatedUserId);
        var result = await commandService.Handle(command);
        return DeleteNutritionLogEntryResultAssembler.ToActionResult(result, localizer, Request.Path);
    }

    [HttpPost]
    [Consumes("application/json")]
    [SwaggerOperation("Log a meal to the daily nutrition log")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LogMeal([FromBody] LogMealResource resource)
    {
        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd", out _))
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd." });

        var command = LogMealCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return LogMealResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("scan-menu/select")]
    [Consumes("application/json")]
    [SwaggerOperation("Select a menu option and persist it to the nutrition log")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SelectMenuOption([FromBody] SelectMenuOptionResource resource)
    {
        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd", out _))
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd." });

        var command = SelectMenuOptionCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return SelectMenuOptionResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("scan-menu")]
    [Consumes("application/json")]
    [SwaggerOperation("Analyze a menu photo and return available options (does not persist)")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ScanMenu([FromBody] ScanPhotoResource resource)
    {
        var command = new ScanMenuPhotoCommand(resource.UserId, resource.ImageBase64OrUri);
        var result = await commandService.Handle(command);
        return MenuOptionsPreviewAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("scan-dish/confirm")]
    [Consumes("application/json")]
    [SwaggerOperation("Confirm and persist a scanned dish to the nutrition log")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmScan([FromBody] ConfirmScanResource resource)
    {
        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd", out _))
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd." });

        var command = ConfirmScanCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return ConfirmScanResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("scan-dish")]
    [Consumes("application/json")]
    [SwaggerOperation("Analyze a dish photo and return a preview (does not persist)")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ScanDish([FromBody] ScanPhotoResource resource)
    {
        var command = new ScanMealPhotoCommand(resource.UserId, resource.ImageBase64OrUri);
        var result = await commandService.Handle(command);
        return ScanPreviewResultAssembler.ToActionResult(result, localizer);
    }
}
