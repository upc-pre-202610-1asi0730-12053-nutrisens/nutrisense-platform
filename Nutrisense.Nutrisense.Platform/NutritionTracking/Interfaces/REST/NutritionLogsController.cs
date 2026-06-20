using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/nutrition-logs")]
[Tags("Nutrition Logs")]
[Authorize]
[Produces("application/json")]
public class NutritionLogsController(
    INutritionLogCommandService commandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
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
