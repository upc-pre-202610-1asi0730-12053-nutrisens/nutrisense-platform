using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST;

[ApiController]
[Route("api/v1/recipes")]
[Tags("Recipes")]
[Produces("application/json")]
public class RecipesController(
    IRecsEngineCommandService commandService,
    IRecsEngineQueryService queryService,
    IStringLocalizer<SmartRecommendationsMessages> localizer) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get recipes by goal", Description = "Retrieve recipes filtered by goal and optional prep time.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recipes retrieved successfully.", typeof(RecipeResource[]))]
    public async Task<IActionResult> GetByGoal([FromQuery] string goal = "weight-loss", [FromQuery] int? maxPrepMinutes = null)
    {
        var recipes = await queryService.Handle(new GetRecipesByGoalQuery(goal, maxPrepMinutes));
        return Ok(recipes.Select(RecipeAssembler.ToResource));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get recipe by ID", Description = "Retrieve a specific recipe with ingredients.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recipe retrieved successfully.", typeof(RecipeResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No recipe was found with the specified id.")]
    public async Task<IActionResult> GetById(int id)
    {
        var recipe = await queryService.Handle(new GetRecipeByIdQuery(id));
        return recipe is null ? NotFound() : Ok(RecipeAssembler.ToResource(recipe));
    }

    [HttpPost("suggest/{userId:int}")]
    [Authorize]
    [SwaggerOperation(Summary = "Suggest recipe", Description = "Suggest a recipe for a user based on their goal and pantry.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recipe suggested successfully.", typeof(RecipeResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "The authenticated user does not match the requested user.")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "A recipe could not be suggested for this user.", typeof(ProblemDetails))]
    public async Task<IActionResult> Suggest(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();

        var result = await commandService.Handle(new SuggestRecipeCommand(userId));
        return result.Fold(
            recipe => (IActionResult)Ok(RecipeAssembler.ToResource(recipe)),
            error => SmartRecommendationsActionResultAssembler.ToActionResult(error, localizer, HttpContext.Request.Path));
    }
}
