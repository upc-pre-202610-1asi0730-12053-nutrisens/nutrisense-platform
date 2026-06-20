using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.Errors;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class UpdateProfileResultAssembler
{
    public static IActionResult ToActionResult(
        Result<User, UpdateProfileError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<User, UpdateProfileError>.Success s =>
                new OkObjectResult(UserResourceAssembler.ToResource(s.Value)),
            Result<User, UpdateProfileError>.Failure { Error: UpdateProfileError.UserNotFound } =>
                new NotFoundObjectResult(new ErrorResponse(localizer["UserNotFound"].Value)),
            Result<User, UpdateProfileError>.Failure { Error: UpdateProfileError.InvalidData } =>
                new BadRequestObjectResult(new ErrorResponse(localizer["InvalidProfileData"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
