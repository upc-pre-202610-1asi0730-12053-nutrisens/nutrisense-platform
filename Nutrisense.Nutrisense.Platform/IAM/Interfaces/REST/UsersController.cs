using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.IAM.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Extensions;
using Nutrisense.Nutrisense.Platform.IAM.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/users")]
[Authorize]
[Tags("Users")]
[Produces("application/json")]
[Consumes("application/json")]
public class UsersController(
    IUserCommandService commandService,
    IUserQueryService queryService,
    IStringLocalizer<IAMMessages> localizer) : ControllerBase
{
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Returns the user profile for the specified user ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User profile retrieved successfully.", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No user was found with the specified id.", typeof(ProblemDetails))]
    public async Task<IActionResult> GetById(int id)
    {
        if (id != this.GetAuthenticatedUserId()) return Forbid();
        var user = await queryService.Handle(new GetUserByIdQuery(new UserId(id)));
        return user is null ? NotFound() : Ok(UserResourceAssembler.ToResource(user));
    }

    [HttpGet("by-email")]
    [SwaggerOperation(Summary = "Check if email is registered", Description = "Returns 200 if the email is already registered, 404 if it is not.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Email is already registered.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No user was found with the specified email address.")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var user = await queryService.Handle(new GetUserByEmailQuery(email));
        return user is null ? NotFound() : Ok();
    }

    [HttpPut("{id:int}/health-goal")]
    [SwaggerOperation(Summary = "Set user health goal", Description = "Updates the user's health goal/intent for nutrition tracking.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Health goal updated successfully. Returns the updated user profile.", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The selected health goal is not valid.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No user was found with the specified id.", typeof(ProblemDetails))]
    public async Task<IActionResult> SetHealthGoal(int id, [FromBody] SetHealthGoalResource resource)
    {
        if (id != this.GetAuthenticatedUserId()) return Forbid();
        var command = SetHealthGoalCommandAssembler.ToCommand(new UserId(id), resource);
        var result = await commandService.Handle(command);
        return IamActionResultAssembler.ToSetHealthGoalResult(result, localizer);
    }

    [HttpPut("{id:int}/dietary-restrictions")]
    [SwaggerOperation(Summary = "Set dietary restrictions", Description = "Replaces all dietary restrictions with the provided list.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Dietary restrictions updated successfully. Returns the updated user profile.", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No user was found with the specified id.", typeof(ProblemDetails))]
    public async Task<IActionResult> SetDietaryRestrictions(int id, [FromBody] SetDietaryRestrictionsResource resource)
    {
        if (id != this.GetAuthenticatedUserId()) return Forbid();
        var command = SetDietaryRestrictionsCommandAssembler.ToCommand(new UserId(id), resource);
        var result = await commandService.Handle(command);
        return IamActionResultAssembler.ToSetDietaryRestrictionsResult(result, localizer);
    }

    [HttpPut("{id:int}/profile")]
    [SwaggerOperation(Summary = "Update user profile", Description = "Updates profile information including name, date of birth, physical measurements, and preferences.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Profile updated successfully. Returns the updated user profile.", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided profile data is not valid.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No user was found with the specified id.", typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileResource resource)
    {
        if (id != this.GetAuthenticatedUserId()) return Forbid();
        var command = UpdateProfileCommandAssembler.ToCommand(new UserId(id), resource);
        var result = await commandService.Handle(command);
        return IamActionResultAssembler.ToUpdateProfileResult(result, localizer);
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete user account", Description = "Permanently removes the user account and all associated data.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User account deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No user was found with the specified id.", typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (id != this.GetAuthenticatedUserId()) return Forbid();
        var result = await commandService.Handle(new DeleteUserCommand(new UserId(id)));
        return IamActionResultAssembler.ToDeleteUserResult(result, localizer);
    }

    [HttpGet("{id:int}/dietary-restrictions")]
    [SwaggerOperation(Summary = "List dietary restrictions", Description = "Returns all dietary restrictions configured for the user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Dietary restrictions retrieved successfully.", typeof(string[]))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetDietaryRestrictions(int id)
    {
        if (id != this.GetAuthenticatedUserId()) return Forbid();
        var restrictions = await queryService.Handle(new GetDietaryRestrictionsByUserIdQuery(new UserId(id)));
        return Ok(restrictions.Select(dr => dr.Restriction).ToArray());
    }
}
