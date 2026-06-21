using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.Errors;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.IAM.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class DeleteUserResultAssembler
{
    public static IActionResult ToActionResult(
        Result<bool, DeleteUserError> result,
        IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<bool, DeleteUserError>.Success =>
                new NoContentResult(),
            Result<bool, DeleteUserError>.Failure { Error: DeleteUserError.UserNotFound } =>
                new NotFoundObjectResult(new ErrorResponse(localizer["UserNotFound"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
