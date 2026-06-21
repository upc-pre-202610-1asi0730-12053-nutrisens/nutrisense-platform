using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class SelectMenuOptionResultAssembler
{
    public static IActionResult ToActionResult(
        Result<NutritionLog, SelectMenuOptionError> result,
        IStringLocalizer<NutritionTrackingMessages> localizer) =>
        result switch
        {
            Result<NutritionLog, SelectMenuOptionError>.Success s =>
                new ObjectResult(NutritionLogResourceAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<NutritionLog, SelectMenuOptionError>.Failure { Error: SelectMenuOptionError.FoodNotFound } =>
                new NotFoundObjectResult(new ErrorResponse(localizer["FoodNotFound"].Value)),
            Result<NutritionLog, SelectMenuOptionError>.Failure { Error: SelectMenuOptionError.InvalidData } =>
                new BadRequestObjectResult(new ErrorResponse(localizer["InvalidMenuSelectionData"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
