using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/recommendations")]
[Tags("Recommendations")]
[Authorize]
[Produces("application/json")]
public class RecommendationsController(
    IRecsEngineCommandService commandService,
    IRecsEngineQueryService queryService) : ControllerBase
{
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get active recommendations", Description = "Retrieve active recommendation cards for a user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveByUser(int userId)
    {
        var cards = await queryService.Handle(new GetActiveRecommendationsByUserIdQuery(userId));
        return Ok(cards.Select(RecommendationCardAssembler.ToResource));
    }

    [HttpPost("generate/{userId:int}")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Generate recommendation", Description = "Generate a new recommendation card for a user based on trigger.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Generate(int userId, [FromBody] GenerateRecommendationResource resource)
    {
        var result = await commandService.Handle(new GenerateRecommendationCommand(userId, resource.Trigger));
        return result.Fold(
            card => (IActionResult)Ok(RecommendationCardAssembler.ToResource(card)),
            error => UnprocessableEntity(new { error = error.ToString() }));
    }
}
