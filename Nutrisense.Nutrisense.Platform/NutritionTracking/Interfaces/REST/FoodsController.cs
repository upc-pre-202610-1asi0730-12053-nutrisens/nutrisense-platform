using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;
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
    IFoodQueryService queryService,
    IFoodImportCommandService importService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation("Search food catalog by name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchFood([FromQuery] string query = "", [FromQuery] string language = "en")
    {
        var foods = await queryService.Handle(new SearchFoodQuery(query, language));
        return Ok(foods.Select(FoodResourceAssembler.ToSearchResource));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [SwaggerOperation("Get a food entry by ID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var food = await queryService.Handle(new GetFoodByIdQuery(id));
        return food is null ? NotFound() : Ok(FoodResourceAssembler.ToResource(food));
    }

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

    [HttpPost("import")]
    [Authorize] // TODO: restrict to admin role once a roles system exists.
    [Consumes("application/json")]
    [SwaggerOperation("Import foods from USDA FoodData Central into the local catalog (admin)")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ImportFoods([FromBody] ImportFoodsResource resource)
    {
        var command = new ImportFoodsCommand(resource.Query, resource.MaxResults, resource.DataType);
        var result = await importService.Handle(command);
        return result.Fold<IActionResult>(
            imported => Ok(new { imported }),
            error => error == ImportFoodsError.UsdaUnavailable
                ? StatusCode(StatusCodes.Status502BadGateway, new { error = "USDA unavailable" })
                : BadRequest(new { error = error.ToString() }));
    }
}
