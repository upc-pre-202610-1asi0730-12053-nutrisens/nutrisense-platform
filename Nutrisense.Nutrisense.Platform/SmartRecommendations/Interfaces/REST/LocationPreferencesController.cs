using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/location-preferences")]
[Tags("Location Preferences")]
[Authorize]
[Produces("application/json")]
public class LocationPreferencesController(
    IRecsEngineCommandService commandService,
    IRecsEngineQueryService queryService) : ControllerBase
{
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get user location", Description = "Retrieve location preferences for a user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var lp = await queryService.Handle(new GetLocationPreferenceByUserIdQuery(userId));
        return lp is null ? NotFound() : Ok(LocationPreferenceAssembler.ToResource(lp));
    }

    [HttpPut("{userId:int}/travel-mode/enable")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Enable travel mode", Description = "Enable travel mode for a user in a specific city.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnableTravelMode(int userId, [FromBody] EnableTravelModeResource resource)
    {
        var result = await commandService.Handle(new EnableTravelModeCommand(userId, resource.CityId));
        return result.Fold(
            lp => (IActionResult)Ok(LocationPreferenceAssembler.ToResource(lp)),
            error => error switch
            {
                Application.Errors.EnableTravelModeError.PlanNotSufficient =>
                    StatusCode(StatusCodes.Status402PaymentRequired, new { error = error.ToString() }),
                Application.Errors.EnableTravelModeError.CityNotFound => NotFound(new { error = error.ToString() }),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            });
    }

    [HttpPut("{userId:int}/travel-mode/disable")]
    [SwaggerOperation(Summary = "Disable travel mode", Description = "Disable travel mode for a user and return to home city.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisableTravelMode(int userId)
    {
        var result = await commandService.Handle(new DisableTravelModeCommand(userId));
        return result.Fold(
            lp => (IActionResult)Ok(LocationPreferenceAssembler.ToResource(lp)),
            error => error switch
            {
                Application.Errors.DisableTravelModeError.LocationNotFound => NotFound(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            });
    }

    [HttpPost("{userId:int}/detect")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Detect location", Description = "Detect and set a user's current location from coordinates.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DetectLocation(int userId, [FromBody] DetectLocationResource resource)
    {
        var result = await commandService.Handle(new DetectLocationCommand(userId, resource.Lat, resource.Lng));
        return result.Fold(
            lp => (IActionResult)Ok(LocationPreferenceAssembler.ToResource(lp)),
            error => UnprocessableEntity(new { error = error.ToString() }));
    }

    [HttpPut("{userId:int}/home-city")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Set home city", Description = "Set the home city for a user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetHomeCity(int userId, [FromBody] SetHomeCityResource resource)
    {
        var result = await commandService.Handle(new SetHomeCityCommand(userId, resource.CityId));
        return result.Fold(
            lp => (IActionResult)Ok(LocationPreferenceAssembler.ToResource(lp)),
            error => error switch
            {
                Application.Errors.SetHomeCityError.CityNotFound => NotFound(new { error = error.ToString() }),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            });
    }
}
