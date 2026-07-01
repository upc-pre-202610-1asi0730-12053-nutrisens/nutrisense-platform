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
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Extensions;
using Nutrisense.Nutrisense.Platform.IAM.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/users/{userId:int}/sessions")]
[Authorize]
[Tags("Sessions")]
[Produces("application/json")]
public class SessionsController(
    IUserCommandService commandService,
    IUserQueryService queryService,
    IStringLocalizer<IAMMessages> localizer) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List user sessions", Description = "Returns all active and inactive sessions for the authenticated user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sessions retrieved successfully.", typeof(SessionResource[]))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetSessions(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var sessions = await queryService.Handle(new GetAllSessionsByUserIdQuery(new UserId(userId)));
        return Ok(sessions.Select(SessionResourceAssembler.ToResource).ToArray());
    }

    [HttpPost("{sessionId:int}/logout")]
    [SwaggerOperation(Summary = "Logout from session", Description = "Terminates a specific session, invalidating its authentication token.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Session terminated successfully.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested session does not exist.", typeof(ProblemDetails))]
    public async Task<IActionResult> Logout(int userId, int sessionId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new LogoutUserCommand(new UserId(userId), sessionId));
        return IamActionResultAssembler.ToLogoutResult(result, localizer);
    }
}
