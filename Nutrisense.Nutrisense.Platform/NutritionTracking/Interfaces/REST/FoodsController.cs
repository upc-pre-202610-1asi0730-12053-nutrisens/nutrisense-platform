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
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
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
    [SwaggerResponse(StatusCodes.Status200OK, "Foods matching the query retrieved successfully.", typeof(FoodSearchResultResource[]))]
    public async Task<IActionResult> SearchFood([FromQuery] string query = "", [FromQuery] string language = "en")
    {
        var foods = await queryService.Handle(new SearchFoodQuery(query, language));
        return Ok(foods.Select(FoodResourceAssembler.ToSearchResource));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [SwaggerOperation("Get a food entry by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Food retrieved successfully.", typeof(FoodResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No food was found with the specified id.")]
    public async Task<IActionResult> GetById(int id)
    {
        var food = await queryService.Handle(new GetFoodByIdQuery(id));
        return food is null ? NotFound() : Ok(FoodResourceAssembler.ToResource(food));
    }

    [HttpPost]
    [Authorize] // TODO: restrict to admin role once a roles system exists.
    [Consumes("application/json")]
    [SwaggerOperation("Register a new food in the catalog (admin)")]
    [SwaggerResponse(StatusCodes.Status201Created, "Food registered successfully.", typeof(FoodResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The food source is invalid.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A food with this key already exists.", typeof(ErrorResponse))]
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
    [SwaggerResponse(StatusCodes.Status200OK, "Foods imported successfully. Returns the number of imported foods.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The import request is not valid.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status502BadGateway, "The USDA food data service is currently unavailable. Please try again later.", typeof(ErrorResponse))]
    public async Task<IActionResult> ImportFoods([FromBody] ImportFoodsResource resource)
    {
        var command = new ImportFoodsCommand(resource.Query, resource.MaxResults, resource.DataType);
        var result = await importService.Handle(command);
        return result.Fold<IActionResult>(
            imported => Ok(new { imported }),
            error => error == ImportFoodsError.UsdaUnavailable
                ? StatusCode(StatusCodes.Status502BadGateway, new ErrorResponse("The USDA food data service is currently unavailable. Please try again later."))
                : BadRequest(new ErrorResponse("The food import request could not be processed.")));
    }
}
