using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.Internal;
using Nutrisense.Nutrisense.Platform.IAM.Application.Errors;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.IAM.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class LoginUserResultAssembler
{
    public static IActionResult ToActionResult(
        Result<LoginResult, LoginUserError> result,
        IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<LoginResult, LoginUserError>.Success s =>
                new OkObjectResult(new LoginResponseResource(s.Value.UserId, s.Value.Token, s.Value.SessionId)),
            Result<LoginResult, LoginUserError>.Failure { Error: LoginUserError.UserNotFound } =>
                new NotFoundObjectResult(new ErrorResponse(localizer["UserNotFound"].Value)),
            Result<LoginResult, LoginUserError>.Failure { Error: LoginUserError.InvalidCredentials } =>
                new UnauthorizedObjectResult(new ErrorResponse(localizer["InvalidCredentials"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
