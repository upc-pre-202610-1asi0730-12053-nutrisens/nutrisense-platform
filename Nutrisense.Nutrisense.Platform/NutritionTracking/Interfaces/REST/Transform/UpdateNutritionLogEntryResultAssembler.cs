using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class UpdateNutritionLogEntryResultAssembler
{
    public static IActionResult ToActionResult(
        Result<NutritionLog, UpdateNutritionLogEntryError> result,
        IStringLocalizer<SharedResource> localizer,
        string instance) =>
        result switch
        {
            Result<NutritionLog, UpdateNutritionLogEntryError>.Success s =>
                new OkObjectResult(NutritionLogResourceAssembler.ToResource(s.Value)),

            Result<NutritionLog, UpdateNutritionLogEntryError>.Failure { Error: UpdateNutritionLogEntryError.EntryNotFound } =>
                new ObjectResult(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = localizer["NutritionLogEntryNotFoundTitle"].Value,
                    Status = StatusCodes.Status404NotFound,
                    Detail = localizer["NutritionLogEntryNotFound"].Value,
                    Instance = instance
                })
                { StatusCode = StatusCodes.Status404NotFound },

            Result<NutritionLog, UpdateNutritionLogEntryError>.Failure { Error: UpdateNutritionLogEntryError.Unauthorized } =>
                new ObjectResult(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = localizer["Unauthorized"].Value,
                    Status = StatusCodes.Status403Forbidden,
                    Detail = localizer["NutritionLogEntryUnauthorized"].Value,
                    Instance = instance
                })
                { StatusCode = StatusCodes.Status403Forbidden },

            Result<NutritionLog, UpdateNutritionLogEntryError>.Failure { Error: UpdateNutritionLogEntryError.InvalidQuantity } =>
                new BadRequestObjectResult(new { message = localizer["NutritionLogEntryInvalidQuantity"].Value }),

            _ =>
                new ObjectResult(new { message = localizer["UnexpectedError"].Value })
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
