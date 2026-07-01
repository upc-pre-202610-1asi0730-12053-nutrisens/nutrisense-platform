using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST;

[ApiController]
[Route("api/v1/foods")]
[Tags("Foods")]
[Produces("application/json")]
public class FoodsController(IFoodQueryService queryService) : ControllerBase
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
}
