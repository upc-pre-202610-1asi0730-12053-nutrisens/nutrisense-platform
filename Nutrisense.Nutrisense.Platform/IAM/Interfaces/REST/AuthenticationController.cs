using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.IAM.Resources;
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
    IStringLocalizer<IAMMessages> localizer) : ControllerBase
{
    [HttpPost("sign-up")]
    [SwaggerOperation(Summary = "Register a new user account", Description = "Creates a new user account with email, password, and initial profile information.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Account created successfully. Returns the newly created user profile.", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The email address format is invalid or the password does not meet the security requirements.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "An account with this email address already exists.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected server error occurred while creating the account.", typeof(ProblemDetails))]
    public async Task<IActionResult> SignUp([FromBody] RegisterUserResource resource)
    {
        var command = RegisterUserCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return IamActionResultAssembler.ToRegisterResult(result, localizer);
    }

    [HttpPost("sign-in")]
    [SwaggerOperation(Summary = "Authenticate user", Description = "Verifies credentials and returns authentication token and session ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Authenticated successfully. Returns the user id, JWT token and session id.", typeof(LoginResponseResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The request body is invalid or missing required fields.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The email or password is incorrect.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No account exists with the provided email address.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected server error occurred while authenticating.", typeof(ProblemDetails))]
    public async Task<IActionResult> SignIn([FromBody] LoginUserResource resource)
    {
        var command = LoginUserCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return IamActionResultAssembler.ToLoginResult(result, localizer);
    }

    [HttpPost("forgot-password")]
    [SwaggerOperation(Summary = "Request a password reset", Description = "Sends a password reset link to the provided email address if it is registered. Always returns 200 to avoid revealing whether the email exists.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request accepted. If the email is registered, a reset link will be sent.")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordResource resource)
    {
        var command = PasswordResetCommandAssembler.ToCommand(resource);
        await commandService.Handle(command);
        return Ok(new { message = localizer["PasswordResetEmailSent"].Value });
    }

    [HttpPost("reset-password")]
    [SwaggerOperation(Summary = "Reset password", Description = "Sets a new password for the user identified by the reset token. The token must be valid and not expired.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Password changed successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The reset token is invalid or expired, or the new password is too weak.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected server error occurred while resetting the password.", typeof(ProblemDetails))]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordResource resource)
    {
        var command = PasswordResetCommandAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return IamActionResultAssembler.ToResetPasswordResult(result, localizer);
    }
}
