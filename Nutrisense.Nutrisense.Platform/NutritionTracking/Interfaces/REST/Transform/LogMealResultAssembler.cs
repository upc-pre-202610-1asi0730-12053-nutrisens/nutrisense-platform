using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class LogMealResultAssembler
{
    public static IActionResult ToActionResult(
        Result<NutritionLog, LogMealError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<NutritionLog, LogMealError>.Success s =>
                new ObjectResult(NutritionLogResourceAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<NutritionLog, LogMealError>.Failure { Error: LogMealError.FoodNotFound } =>
                new NotFoundObjectResult(new ErrorResponse("Food not found.")),
            Result<NutritionLog, LogMealError>.Failure { Error: LogMealError.InvalidMealType } =>
                new BadRequestObjectResult(new ErrorResponse("Invalid meal type.")),
            Result<NutritionLog, LogMealError>.Failure { Error: LogMealError.InvalidQuantity } =>
                new BadRequestObjectResult(new ErrorResponse("Invalid quantity.")),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
