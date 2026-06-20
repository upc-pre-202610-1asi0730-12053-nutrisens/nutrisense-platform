using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;

/// <summary>Converts a sync-activity <see cref="Result{TValue,TError}"/> into the matching HTTP action result.</summary>
public static class SyncActivityResultAssembler
{
    /// <summary>Maps the sync-activity result to a 200/404/400/500 response.</summary>
    /// <param name="result">The outcome of the sync-activity-data command.</param>
    /// <param name="localizer">Localizer used for the fallback error message.</param>
    /// <returns>The corresponding <see cref="IActionResult"/>.</returns>
    public static IActionResult ToActionResult(
        Result<WearableConnection, SyncActivityDataError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<WearableConnection, SyncActivityDataError>.Success s =>
                new OkObjectResult(WearableConnectionAssembler.ToResource(s.Value)),
            Result<WearableConnection, SyncActivityDataError>.Failure { Error: SyncActivityDataError.ConnectionNotFound } =>
                new NotFoundObjectResult(new ErrorResponse("Wearable connection not found.")),
            Result<WearableConnection, SyncActivityDataError>.Failure { Error: SyncActivityDataError.SyncFailed } =>
                new BadRequestObjectResult(new ErrorResponse("Activity sync failed.")),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
