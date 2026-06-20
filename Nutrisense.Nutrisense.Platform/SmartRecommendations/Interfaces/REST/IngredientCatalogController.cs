using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST;

[ApiController]
[Route("api/v1/ingredient-catalog")]
[Tags("Ingredient Catalog")]
[AllowAnonymous]
[Produces("application/json")]
public class IngredientCatalogController(IRecsEngineQueryService queryService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List all ingredients", Description = "Retrieve the full ingredient catalog.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Ingredient catalog retrieved successfully.", typeof(IngredientCatalogItemResource[]))]
    public async Task<IActionResult> GetAll()
    {
        var items = await queryService.Handle(new GetIngredientCatalogQuery());
        return Ok(items.Select(IngredientCatalogAssembler.ToResource));
    }
}
