using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/wearable-connections")]
[Authorize]
[Tags("Wearable Connections")]
[Produces("application/json")]
[Consumes("application/json")]
/// <summary>REST endpoints for connecting, querying, syncing and disconnecting user wearable devices.</summary>
public class WearableConnectionsController(
    IWearableConnectionCommandService commandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Connects a wearable device and triggers the full activity sync chain.</summary>
    /// <param name="resource">The connect payload carrying the user, provider and OAuth code.</param>
    /// <returns>201 Created with the connection, 400 Bad Request on invalid input, or 409 Conflict if already connected.</returns>
    [HttpPost]
    [SwaggerOperation("Connect a wearable device and trigger full sync chain (Subflow 5.1)")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConnectDevice([FromBody] ConnectDeviceResource resource)
    {
        var command = WearableConnectionAssembler.ToCommand(resource);
        var result = await commandService.Handle(command);
        return ConnectDeviceResultAssembler.ToActionResult(result, localizer);
    }

    /// <summary>Manually triggers an activity data sync for a wearable connection.</summary>
    /// <param name="id">Identifier of the wearable connection to sync.</param>
    /// <returns>200 OK with the updated connection, or 404 Not Found if it does not exist.</returns>
    [HttpPost("{id:int}/sync")]
    [SwaggerOperation("Manually trigger activity data sync for a wearable connection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SyncActivityData(int id)
    {
        var result = await commandService.Handle(new SyncActivityDataCommand(id));
        return SyncActivityResultAssembler.ToActionResult(result, localizer);
    }
}
