using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class RegisterFoodResultAssembler
{
    public static IActionResult ToActionResult(
        Result<Food, RegisterFoodError> result,
        IStringLocalizer<NutritionTrackingMessages> localizer) =>
        result switch
        {
            Result<Food, RegisterFoodError>.Success s =>
                new ObjectResult(FoodResourceAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<Food, RegisterFoodError>.Failure { Error: RegisterFoodError.DuplicateKey } =>
                new ConflictObjectResult(new ErrorResponse(localizer["FoodDuplicateKey"].Value)),
            Result<Food, RegisterFoodError>.Failure { Error: RegisterFoodError.InvalidSource } =>
                new BadRequestObjectResult(new ErrorResponse(localizer["InvalidFoodSource"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
