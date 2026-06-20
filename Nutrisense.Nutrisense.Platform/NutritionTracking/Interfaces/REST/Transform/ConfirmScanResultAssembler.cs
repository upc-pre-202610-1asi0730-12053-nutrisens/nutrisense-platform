using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class ConfirmScanResultAssembler
{
    public static IActionResult ToActionResult(
        Result<NutritionLog, ConfirmScanError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<NutritionLog, ConfirmScanError>.Success s =>
                new ObjectResult(NutritionLogResourceAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<NutritionLog, ConfirmScanError>.Failure { Error: ConfirmScanError.FoodNotFound } =>
                new NotFoundObjectResult(new ErrorResponse("Food not found.")),
            Result<NutritionLog, ConfirmScanError>.Failure { Error: ConfirmScanError.InvalidData } =>
                new BadRequestObjectResult(new ErrorResponse("Invalid scan confirmation data.")),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
