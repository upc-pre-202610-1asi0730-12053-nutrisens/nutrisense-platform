using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Errors;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST;

[ApiController]
[Route("api/v1/recipes")]
[Tags("Recipes")]
[Produces("application/json")]
public class RecipesController(
    IRecsEngineCommandService commandService,
    IRecsEngineQueryService queryService,
    IRecipeImportCommandService importService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get recipes by goal", Description = "Retrieve recipes filtered by goal and optional prep time.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGoal([FromQuery] string goal = "weight-loss", [FromQuery] int? maxPrepMinutes = null)
    {
        var recipes = await queryService.Handle(new GetRecipesByGoalQuery(goal, maxPrepMinutes));
        return Ok(recipes.Select(RecipeAssembler.ToResource));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get recipe by ID", Description = "Retrieve a specific recipe with ingredients.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var recipe = await queryService.Handle(new GetRecipeByIdQuery(id));
        return recipe is null ? NotFound() : Ok(RecipeAssembler.ToResource(recipe));
    }

    // TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
    [HttpPost("suggest/{userId:int}")]
    [Authorize]
    [SwaggerOperation(Summary = "Suggest recipe", Description = "Suggest a recipe for a user based on their goal and pantry.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Suggest(int userId)
    {
        var result = await commandService.Handle(new SuggestRecipeCommand(userId));
        return result.Fold(
            recipe => (IActionResult)Ok(RecipeAssembler.ToResource(recipe)),
            error => UnprocessableEntity(new { error = error.ToString() }));
    }

    [HttpPost("import")]
    [Authorize] // TODO: restrict to admin role once a roles system exists.
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Import AI recipes", Description = "Generate and import recipe suggestions from the ingredient catalog (admin only).")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Import([FromBody] ImportRecipesResource resource)
    {
        var command = new ImportRecipeSuggestionsCommand(resource.GoalTypes, resource.MaxPerGoal);
        var result = await importService.Handle(command);
        return result.Fold<IActionResult>(
            generated => Ok(new { generated }),
            error => error == RecipeImportError.InsufficientIngredients
                ? UnprocessableEntity(new { error = "Insufficient ingredients; import foods first." })
                : BadRequest(new { error = error.ToString() }));
    }
}
