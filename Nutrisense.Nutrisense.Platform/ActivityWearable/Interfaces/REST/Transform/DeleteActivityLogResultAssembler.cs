using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Resources;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;

/// <summary>Converts a delete-activity-log <see cref="Result{TValue,TError}"/> into the matching HTTP action result.</summary>
public static class DeleteActivityLogResultAssembler
{
    /// <summary>Maps the delete result to a 204/404/403/500 response, using localized problem details.</summary>
    /// <param name="result">The outcome of the delete-activity-log command.</param>
    /// <param name="localizer">Localizer used for the problem-detail titles and messages.</param>
    /// <returns>The corresponding <see cref="IActionResult"/>.</returns>
    public static IActionResult ToActionResult(
        Result<bool, DeleteActivityLogError> result,
        IStringLocalizer<ActivityWearableMessages> localizer) =>
        result switch
        {
            Result<bool, DeleteActivityLogError>.Success =>
                new NoContentResult(),

            Result<bool, DeleteActivityLogError>.Failure { Error: DeleteActivityLogError.NotFound } =>
                new NotFoundObjectResult(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = localizer["ActivityLogNotFoundTitle"].Value,
                    Status = StatusCodes.Status404NotFound,
                    Detail = localizer["ActivityLogNotFound"].Value
                }),

            Result<bool, DeleteActivityLogError>.Failure { Error: DeleteActivityLogError.NotOwner } =>
                new ObjectResult(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = localizer["Forbidden"].Value,
                    Status = StatusCodes.Status403Forbidden,
                    Detail = localizer["ActivityLogDeleteNotOwner"].Value
                }) { StatusCode = StatusCodes.Status403Forbidden },

            _ =>
                new ObjectResult(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = localizer["UnexpectedServerError"].Value,
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = localizer["UnexpectedError"].Value
                }) { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
