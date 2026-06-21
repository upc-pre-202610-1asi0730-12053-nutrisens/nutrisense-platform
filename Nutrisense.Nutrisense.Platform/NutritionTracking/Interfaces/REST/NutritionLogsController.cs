using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Resources;
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
    IStringLocalizer<NutritionTrackingMessages> localizer) : ControllerBase
{
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation("Get all nutrition log entries for a user on a specific date")]
    [SwaggerResponse(StatusCodes.Status200OK, "Nutrition log entries retrieved successfully.", typeof(NutritionLogResource[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided date is not in the expected yyyy-MM-dd format.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetByUserAndDate(int userId, [FromQuery] string date)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(new ErrorResponse(localizer["InvalidDateFormat"].Value));

        var logs = await queryService.Handle(new GetNutritionLogByUserAndDateQuery(userId, parsedDate));
        return Ok(logs.Select(NutritionLogResourceAssembler.ToResource));
    }

    [HttpPatch("{entryId:int}")]
    [Consumes("application/json")]
    [SwaggerOperation("Update the quantity of an existing nutrition log entry")]
    [SwaggerResponse(StatusCodes.Status200OK, "Entry updated successfully. Returns the updated nutrition log.", typeof(NutritionLogResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided quantity is not valid.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "You are not allowed to modify this nutrition log entry.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested nutrition log entry does not exist.", typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateEntry(int entryId, [FromBody] UpdateNutritionLogResource resource)
    {
        var command = UpdateNutritionLogEntryCommandAssembler.ToCommand(entryId, resource);
        var result = await commandService.Handle(command);
        return UpdateNutritionLogEntryResultAssembler.ToActionResult(result, localizer, Request.Path);
    }

    [HttpGet("by-user/{userId:int}/daily-summary")]
    [SwaggerOperation("Get the daily macro summary for a user on a specific date")]
    [SwaggerResponse(StatusCodes.Status200OK, "Daily macro summary retrieved successfully.", typeof(DailyMacroSummaryResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided date is not in the expected yyyy-MM-dd format.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetDailySummary(int userId, [FromQuery] string date)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(new ErrorResponse(localizer["InvalidDateFormat"].Value));

        var summary = await queryService.Handle(new GetDailyMacroSummaryQuery(userId, parsedDate));
        return Ok(NutritionLogResourceAssembler.ToSummaryResource(summary));
    }

    [HttpGet("by-user/{userId:int}/history")]
    [SwaggerOperation("Get nutrition log history for a user with optional date range")]
    [SwaggerResponse(StatusCodes.Status200OK, "Nutrition log history retrieved successfully.", typeof(NutritionLogResource[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "One of the provided dates is not in the expected yyyy-MM-dd format.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetHistory(
        int userId,
        [FromQuery] string? from = null,
        [FromQuery] string? to = null)
    {
        DateOnly? fromDate = null;
        DateOnly? toDate = null;

        if (from is not null && !DateOnly.TryParseExact(from, "yyyy-MM-dd", out var parsedFrom))
            return BadRequest(new ErrorResponse(localizer["InvalidFromDateFormat"].Value));
        else if (from is not null)
            fromDate = DateOnly.ParseExact(from, "yyyy-MM-dd", null);

        if (to is not null && !DateOnly.TryParseExact(to, "yyyy-MM-dd", out var parsedTo))
            return BadRequest(new ErrorResponse(localizer["InvalidToDateFormat"].Value));
        else if (to is not null)
            toDate = DateOnly.ParseExact(to, "yyyy-MM-dd", null);

        var logs = await queryService.Handle(new GetNutritionLogsByUserQuery(userId, fromDate, toDate));
        return Ok(logs.Select(NutritionLogResourceAssembler.ToResource));
    }

    [HttpDelete("{entryId:int}")]
    [SwaggerOperation("Delete a nutrition log entry")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Entry deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "You are not allowed to delete this nutrition log entry.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested nutrition log entry does not exist.", typeof(ProblemDetails))]
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
    [SwaggerResponse(StatusCodes.Status201Created, "Meal logged successfully. Returns the created nutrition log.", typeof(NutritionLogResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The date format, meal type or quantity is not valid.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The referenced food was not found.", typeof(ErrorResponse))]
    public async Task<IActionResult> LogMeal([FromBody] LogMealResource resource)
    {
        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd", out _))
            return BadRequest(new ErrorResponse(localizer["InvalidDateFormat"].Value));

        var command = LogMealCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return LogMealResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("scan-menu/select")]
    [Consumes("application/json")]
    [SwaggerOperation("Select a menu option and persist it to the nutrition log")]
    [SwaggerResponse(StatusCodes.Status201Created, "Menu option selected and logged successfully. Returns the created nutrition log.", typeof(NutritionLogResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The date format or menu selection data is not valid.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The referenced food was not found.", typeof(ErrorResponse))]
    public async Task<IActionResult> SelectMenuOption([FromBody] SelectMenuOptionResource resource)
    {
        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd", out _))
            return BadRequest(new ErrorResponse(localizer["InvalidDateFormat"].Value));

        var command = SelectMenuOptionCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return SelectMenuOptionResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("scan-menu")]
    [Consumes("application/json")]
    [SwaggerOperation("Analyze a menu photo and return available options (does not persist)")]
    [SwaggerResponse(StatusCodes.Status200OK, "Menu analyzed successfully. Returns the available options.", typeof(MenuOptionsPreviewResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "The menu photo could not be analyzed.", typeof(ErrorResponse))]
    public async Task<IActionResult> ScanMenu([FromBody] ScanPhotoResource resource)
    {
        var command = new ScanMenuPhotoCommand(resource.UserId, resource.ImageBase64OrUri);
        var result = await commandService.Handle(command);
        return MenuOptionsPreviewAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("scan-dish/confirm")]
    [Consumes("application/json")]
    [SwaggerOperation("Confirm and persist a scanned dish to the nutrition log")]
    [SwaggerResponse(StatusCodes.Status201Created, "Scanned dish confirmed and logged successfully. Returns the created nutrition log.", typeof(NutritionLogResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The date format or scan confirmation data is not valid.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The referenced food was not found.", typeof(ErrorResponse))]
    public async Task<IActionResult> ConfirmScan([FromBody] ConfirmScanResource resource)
    {
        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd", out _))
            return BadRequest(new ErrorResponse(localizer["InvalidDateFormat"].Value));

        var command = ConfirmScanCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return ConfirmScanResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("scan-dish")]
    [Consumes("application/json")]
    [SwaggerOperation("Analyze a dish photo and return a preview (does not persist)")]
    [SwaggerResponse(StatusCodes.Status200OK, "Dish analyzed successfully. Returns the scan preview.", typeof(ScanPreviewResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "The dish photo could not be analyzed.", typeof(ErrorResponse))]
    public async Task<IActionResult> ScanDish([FromBody] ScanPhotoResource resource)
    {
        var command = new ScanMealPhotoCommand(resource.UserId, resource.ImageBase64OrUri);
        var result = await commandService.Handle(command);
        return ScanPreviewResultAssembler.ToActionResult(result, localizer);
    }
}
