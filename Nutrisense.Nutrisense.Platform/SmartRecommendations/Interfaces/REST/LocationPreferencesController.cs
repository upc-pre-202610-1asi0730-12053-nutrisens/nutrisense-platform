using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using SharedProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST;

[ApiController]
[Route("api/v1/location-preferences")]
[Tags("Location Preferences")]
[Authorize]
[Produces("application/json")]
public class LocationPreferencesController(
    IRecsEngineCommandService commandService,
    IRecsEngineQueryService queryService,
    IStringLocalizer<SmartRecommendationsMessages> localizer) : ControllerBase
{
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get user location", Description = "Retrieve location preferences for a user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Location preferences retrieved successfully.", typeof(LocationPreferenceResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No location preferences were found for this user.", typeof(ProblemDetails))]
    public async Task<IActionResult> GetByUser(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var lp = await queryService.Handle(new GetLocationPreferenceByUserIdQuery(userId));
        return lp is null
            ? NotFound(SharedProblemDetailsFactory.Create(StatusCodes.Status404NotFound,
                localizer["NotFoundTitle"].Value, localizer["LocationPreferencesNotFound"].Value, HttpContext.Request.Path))
            : Ok(LocationPreferenceAssembler.ToResource(lp));
    }

    [HttpPut("{userId:int}/travel-mode/enable")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Enable travel mode", Description = "Enable travel mode for a user in a specific city.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Travel mode enabled successfully. Returns the updated location preferences.", typeof(LocationPreferenceResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status402PaymentRequired, "Your current plan does not allow enabling travel mode.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested city was not found.", typeof(ProblemDetails))]
    public async Task<IActionResult> EnableTravelMode(int userId, [FromBody] EnableTravelModeResource resource)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new EnableTravelModeCommand(userId, resource.CityId));
        return result.Fold(
            lp => (IActionResult)Ok(LocationPreferenceAssembler.ToResource(lp)),
            error => SmartRecommendationsActionResultAssembler.ToActionResult(error, localizer, HttpContext.Request.Path));
    }

    [HttpPut("{userId:int}/travel-mode/disable")]
    [SwaggerOperation(Summary = "Disable travel mode", Description = "Disable travel mode for a user and return to home city.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Travel mode disabled successfully. Returns the updated location preferences.", typeof(LocationPreferenceResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No location preferences were found for this user.", typeof(ProblemDetails))]
    public async Task<IActionResult> DisableTravelMode(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new DisableTravelModeCommand(userId));
        return result.Fold(
            lp => (IActionResult)Ok(LocationPreferenceAssembler.ToResource(lp)),
            error => SmartRecommendationsActionResultAssembler.ToActionResult(error, localizer, HttpContext.Request.Path));
    }

    [HttpPost("{userId:int}/detect")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Detect location", Description = "Detect and set a user's current location from coordinates.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Location detected and set successfully. Returns the updated location preferences.", typeof(LocationPreferenceResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "The location could not be detected from the provided coordinates.", typeof(ProblemDetails))]
    public async Task<IActionResult> DetectLocation(int userId, [FromBody] DetectLocationResource resource)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new DetectLocationCommand(userId, resource.Lat, resource.Lng));
        return result.Fold(
            lp => (IActionResult)Ok(LocationPreferenceAssembler.ToResource(lp)),
            error => SmartRecommendationsActionResultAssembler.ToActionResult(error, localizer, HttpContext.Request.Path));
    }

    [HttpPut("{userId:int}/permission")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Set location permission", Description = "Persist the user's intent to use location-based recommendations (account-level).")]
    [SwaggerResponse(StatusCodes.Status200OK, "Location permission updated successfully. Returns the updated location preferences.", typeof(LocationPreferenceResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> SetLocationPermission(int userId, [FromBody] SetLocationPermissionResource resource)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var lp = await commandService.Handle(new SetLocationPermissionCommand(userId, resource.Granted));
        return Ok(LocationPreferenceAssembler.ToResource(lp));
    }

    [HttpPut("{userId:int}/home-city")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Set home city", Description = "Set the home city for a user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Home city set successfully. Returns the updated location preferences.", typeof(LocationPreferenceResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested city was not found.", typeof(ProblemDetails))]
    public async Task<IActionResult> SetHomeCity(int userId, [FromBody] SetHomeCityResource resource)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new SetHomeCityCommand(userId, resource.CityId));
        return result.Fold(
            lp => (IActionResult)Ok(LocationPreferenceAssembler.ToResource(lp)),
            error => SmartRecommendationsActionResultAssembler.ToActionResult(error, localizer, HttpContext.Request.Path));
    }
}
