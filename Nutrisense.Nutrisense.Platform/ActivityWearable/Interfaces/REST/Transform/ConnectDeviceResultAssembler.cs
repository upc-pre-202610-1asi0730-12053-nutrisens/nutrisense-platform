using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;

/// <summary>Converts a connect-device <see cref="Result{TValue,TError}"/> into the matching HTTP action result.</summary>
public static class ConnectDeviceResultAssembler
{
    /// <summary>Maps the connect-device result to a 201/409/400/500 response.</summary>
    /// <param name="result">The outcome of the connect-device command.</param>
    /// <param name="localizer">Localizer used for the fallback error message.</param>
    /// <returns>The corresponding <see cref="IActionResult"/>.</returns>
    public static IActionResult ToActionResult(
        Result<WearableConnection, ConnectDeviceError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<WearableConnection, ConnectDeviceError>.Success s =>
                new ObjectResult(WearableConnectionAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<WearableConnection, ConnectDeviceError>.Failure { Error: ConnectDeviceError.AlreadyConnected } =>
                new ConflictObjectResult(new ErrorResponse("Device already connected for this provider.")),
            Result<WearableConnection, ConnectDeviceError>.Failure { Error: ConnectDeviceError.AuthorizationFailed } =>
                new BadRequestObjectResult(new ErrorResponse("OAuth authorization failed.")),
            Result<WearableConnection, ConnectDeviceError>.Failure { Error: ConnectDeviceError.InvalidProvider } =>
                new BadRequestObjectResult(new ErrorResponse("Invalid wearable provider.")),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
