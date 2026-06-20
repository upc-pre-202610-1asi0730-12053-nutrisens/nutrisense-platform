using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class DeleteNutritionLogEntryResultAssembler
{
    public static IActionResult ToActionResult(
        Result<bool, DeleteNutritionLogEntryError> result,
        IStringLocalizer<SharedResource> localizer,
        string instance) =>
        result switch
        {
            Result<bool, DeleteNutritionLogEntryError>.Success =>
                new NoContentResult(),

            Result<bool, DeleteNutritionLogEntryError>.Failure { Error: DeleteNutritionLogEntryError.EntryNotFound } =>
                new ObjectResult(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = localizer["NutritionLogEntryDeletedTitle"].Value,
                    Status = StatusCodes.Status404NotFound,
                    Detail = localizer["NutritionLogEntryNotFound"].Value,
                    Instance = instance
                })
                { StatusCode = StatusCodes.Status404NotFound },

            Result<bool, DeleteNutritionLogEntryError>.Failure { Error: DeleteNutritionLogEntryError.Unauthorized } =>
                new ObjectResult(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = localizer["Unauthorized"].Value,
                    Status = StatusCodes.Status403Forbidden,
                    Detail = localizer["NutritionLogEntryDeleteUnauthorized"].Value,
                    Instance = instance
                })
                { StatusCode = StatusCodes.Status403Forbidden },

            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
