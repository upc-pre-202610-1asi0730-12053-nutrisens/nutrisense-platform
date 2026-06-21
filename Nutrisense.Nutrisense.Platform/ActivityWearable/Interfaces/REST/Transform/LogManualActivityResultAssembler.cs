using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Resources;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;

/// <summary>Converts a log-manual-activity <see cref="Result{TValue,TError}"/> into the matching HTTP action result.</summary>
public static class LogManualActivityResultAssembler
{
    /// <summary>Maps the log-activity result to a 201/400/500 response.</summary>
    /// <param name="result">The outcome of the log-manual-activity command.</param>
    /// <param name="localizer">Localizer used for the fallback error message.</param>
    /// <returns>The corresponding <see cref="IActionResult"/>.</returns>
    public static IActionResult ToActionResult(
        Result<ActivityLog, LogManualActivityError> result,
        IStringLocalizer<ActivityWearableMessages> localizer) =>
        result switch
        {
            Result<ActivityLog, LogManualActivityError>.Success s =>
                new ObjectResult(ActivityLogAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<ActivityLog, LogManualActivityError>.Failure { Error: LogManualActivityError.InvalidActivity } =>
                new BadRequestObjectResult(new ErrorResponse(localizer["InvalidActivityData"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
