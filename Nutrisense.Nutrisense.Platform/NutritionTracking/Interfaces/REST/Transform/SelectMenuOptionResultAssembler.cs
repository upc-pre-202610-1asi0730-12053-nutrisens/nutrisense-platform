using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class SelectMenuOptionResultAssembler
{
    public static IActionResult ToActionResult(
        Result<NutritionLog, SelectMenuOptionError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<NutritionLog, SelectMenuOptionError>.Success s =>
                new ObjectResult(NutritionLogResourceAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<NutritionLog, SelectMenuOptionError>.Failure { Error: SelectMenuOptionError.FoodNotFound } =>
                new NotFoundObjectResult(new { message = "Food not found." }),
            Result<NutritionLog, SelectMenuOptionError>.Failure { Error: SelectMenuOptionError.InvalidData } =>
                new BadRequestObjectResult(new { message = "Invalid menu selection data." }),
            _ =>
                new ObjectResult(new { message = localizer["UnexpectedError"].Value })
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
