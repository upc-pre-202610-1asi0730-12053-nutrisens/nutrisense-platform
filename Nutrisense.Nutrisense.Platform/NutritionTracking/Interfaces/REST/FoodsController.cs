using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST;

[ApiController]
[Route("api/v1/foods")]
[Tags("Foods")]
[Produces("application/json")]
public class FoodsController(
    IFoodCommandService commandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpPost]
    [Authorize] // TODO: restrict to admin role once a roles system exists.
    [Consumes("application/json")]
    [SwaggerOperation("Register a new food in the catalog (admin)")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterFood([FromBody] RegisterFoodResource resource)
    {
        var command = RegisterFoodCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return RegisterFoodResultAssembler.ToActionResult(result, localizer);
    }
}
