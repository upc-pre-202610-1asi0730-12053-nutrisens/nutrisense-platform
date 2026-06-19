using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/authentication")]
[AllowAnonymous]
[Tags("Authentication")]
[Produces("application/json")]
[Consumes("application/json")]
public class AuthenticationController(
    IUserCommandService commandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpPost("sign-up")]
    [SwaggerOperation(Summary = "Register a new user account", Description = "Creates a new user account with email, password, and initial profile information.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp([FromBody] RegisterUserResource resource)
    {
        var command = RegisterUserCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return RegisterUserResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPost("sign-in")]
    [SwaggerOperation(Summary = "Authenticate user", Description = "Verifies credentials and returns authentication token and session ID.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SignIn([FromBody] LoginUserResource resource)
    {
        var command = LoginUserCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return LoginUserResultAssembler.ToActionResult(result, localizer);
    }
}
