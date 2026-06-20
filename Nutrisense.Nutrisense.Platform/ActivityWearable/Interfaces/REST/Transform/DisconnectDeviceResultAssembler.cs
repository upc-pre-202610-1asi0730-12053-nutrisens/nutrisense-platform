using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;

/// <summary>Converts a disconnect-device <see cref="Result{TValue,TError}"/> into the matching HTTP action result.</summary>
public static class DisconnectDeviceResultAssembler
{
    /// <summary>Maps the disconnect result to a 204/404/500 response.</summary>
    /// <param name="result">The outcome of the disconnect-device command.</param>
    /// <param name="localizer">Localizer used for the fallback error message.</param>
    /// <returns>The corresponding <see cref="IActionResult"/>.</returns>
    public static IActionResult ToActionResult(
        Result<bool, DisconnectDeviceError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<bool, DisconnectDeviceError>.Success =>
                new NoContentResult(),
            Result<bool, DisconnectDeviceError>.Failure { Error: DisconnectDeviceError.ConnectionNotFound } =>
                new NotFoundObjectResult(new ErrorResponse("Wearable connection not found.")),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
