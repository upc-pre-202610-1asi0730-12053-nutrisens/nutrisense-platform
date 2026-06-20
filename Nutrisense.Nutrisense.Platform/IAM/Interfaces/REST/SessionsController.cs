using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.IAM.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/users/{userId:int}/sessions")]
[Authorize]
[Tags("Sessions")]
[Produces("application/json")]
public class SessionsController(
    IUserCommandService commandService,
    IUserQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List user sessions", Description = "Returns all active and inactive sessions for the authenticated user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sessions retrieved successfully.", typeof(SessionResource[]))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetSessions(int userId)
    {
        var sessions = await queryService.Handle(new GetAllSessionsByUserIdQuery(new UserId(userId)));
        return Ok(sessions.Select(SessionResourceAssembler.ToResource).ToArray());
    }

    [HttpPost("{sessionId:int}/logout")]
    [SwaggerOperation(Summary = "Logout from session", Description = "Terminates a specific session, invalidating its authentication token.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Session terminated successfully.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested session does not exist.", typeof(ErrorResponse))]
    public async Task<IActionResult> Logout(int userId, int sessionId)
    {
        var result = await commandService.Handle(new LogoutUserCommand(new UserId(userId), sessionId));
        return LogoutUserResultAssembler.ToActionResult(result, localizer);
    }
}
