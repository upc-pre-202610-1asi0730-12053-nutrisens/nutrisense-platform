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
[Route("api/v1/recommendations")]
[Tags("Recommendations")]
[Authorize]
[Produces("application/json")]
public class RecommendationsController(
    IRecsEngineCommandService commandService,
    IRecsEngineQueryService queryService,
    IStringLocalizer<SmartRecommendationsMessages> localizer) : ControllerBase
{
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get active recommendations", Description = "Retrieve active recommendation cards for a user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Active recommendations retrieved successfully.", typeof(RecommendationCardResource[]))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetActiveByUser(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var cards = await queryService.Handle(new GetActiveRecommendationsByUserIdQuery(userId));
        return Ok(cards.Select(RecommendationCardAssembler.ToResource));
    }

    [HttpPost("generate/{userId:int}")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Generate recommendation", Description = "Generate a new recommendation card for a user based on trigger.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recommendation generated successfully.", typeof(RecommendationCardResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "A recommendation could not be generated for the given trigger.", typeof(ProblemDetails))]
    public async Task<IActionResult> Generate(int userId, [FromBody] GenerateRecommendationResource resource)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new GenerateRecommendationCommand(userId, resource.Trigger));
        return result.Fold(
            card => (IActionResult)Ok(RecommendationCardAssembler.ToResource(card)),
            error => SmartRecommendationsActionResultAssembler.ToActionResult(error, localizer, HttpContext.Request.Path));
    }
}
