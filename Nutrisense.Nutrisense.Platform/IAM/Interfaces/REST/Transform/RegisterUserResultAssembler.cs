using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.Errors;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class RegisterUserResultAssembler
{
    public static IActionResult ToActionResult(
        Result<User, RegisterUserError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<User, RegisterUserError>.Success s =>
                new ObjectResult(UserResourceAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<User, RegisterUserError>.Failure { Error: RegisterUserError.EmailAlreadyTaken } =>
                new ConflictObjectResult(new ErrorResponse(localizer["EmailAlreadyTaken"].Value)),
            Result<User, RegisterUserError>.Failure { Error: RegisterUserError.WeakPassword } =>
                new BadRequestObjectResult(new ErrorResponse(localizer["WeakPassword"].Value)),
            Result<User, RegisterUserError>.Failure { Error: RegisterUserError.InvalidEmail } =>
                new BadRequestObjectResult(new ErrorResponse(localizer["InvalidEmail"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
