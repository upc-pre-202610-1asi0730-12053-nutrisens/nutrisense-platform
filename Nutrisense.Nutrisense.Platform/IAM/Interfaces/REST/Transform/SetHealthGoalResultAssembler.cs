using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.Errors;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class SetHealthGoalResultAssembler
{
    public static IActionResult ToActionResult(
        Result<User, SetHealthGoalError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<User, SetHealthGoalError>.Success s =>
                new OkObjectResult(UserResourceAssembler.ToResource(s.Value)),
            Result<User, SetHealthGoalError>.Failure { Error: SetHealthGoalError.UserNotFound } =>
                new NotFoundObjectResult(new ErrorResponse(localizer["UserNotFound"].Value)),
            Result<User, SetHealthGoalError>.Failure { Error: SetHealthGoalError.InvalidGoal } =>
                new BadRequestObjectResult(new ErrorResponse(localizer["InvalidGoal"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
