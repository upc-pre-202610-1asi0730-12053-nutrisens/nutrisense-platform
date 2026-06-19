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
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST;

// TODO (IDOR): validate that route {id} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/users")]
[Authorize]
[Tags("Users")]
[Produces("application/json")]
[Consumes("application/json")]
public class UsersController(
    IUserCommandService commandService,
    IUserQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Returns the user profile for the specified user ID.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await queryService.Handle(new GetUserByIdQuery(new UserId(id)));
        return user is null ? NotFound() : Ok(UserResourceAssembler.ToResource(user));
    }

    [HttpGet("by-email")]
    [SwaggerOperation(Summary = "Get user by email", Description = "Returns the user profile for the specified email address.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var user = await queryService.Handle(new GetUserByEmailQuery(email));
        return user is null ? NotFound() : Ok(UserResourceAssembler.ToResource(user));
    }

    [HttpPut("{id:int}/health-goal")]
    [SwaggerOperation(Summary = "Set user health goal", Description = "Updates the user's health goal/intent for nutrition tracking.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetHealthGoal(int id, [FromBody] SetHealthGoalResource resource)
    {
        var command = SetHealthGoalCommandAssembler.ToCommand(new UserId(id), resource);
        var result = await commandService.Handle(command);
        return SetHealthGoalResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPut("{id:int}/dietary-restrictions")]
    [SwaggerOperation(Summary = "Set dietary restrictions", Description = "Replaces all dietary restrictions with the provided list.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDietaryRestrictions(int id, [FromBody] SetDietaryRestrictionsResource resource)
    {
        var command = SetDietaryRestrictionsCommandAssembler.ToCommand(new UserId(id), resource);
        var result = await commandService.Handle(command);
        return SetDietaryRestrictionsResultAssembler.ToActionResult(result, localizer);
    }

    [HttpPut("{id:int}/profile")]
    [SwaggerOperation(Summary = "Update user profile", Description = "Updates profile information including name, date of birth, physical measurements, and preferences.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileResource resource)
    {
        var command = UpdateProfileCommandAssembler.ToCommand(new UserId(id), resource);
        var result = await commandService.Handle(command);
        return UpdateProfileResultAssembler.ToActionResult(result, localizer);
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete user account", Description = "Permanently removes the user account and all associated data.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await commandService.Handle(new DeleteUserCommand(new UserId(id)));
        return DeleteUserResultAssembler.ToActionResult(result, localizer);
    }

    [HttpGet("{id:int}/dietary-restrictions")]
    [SwaggerOperation(Summary = "List dietary restrictions", Description = "Returns all dietary restrictions configured for the user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDietaryRestrictions(int id)
    {
        var restrictions = await queryService.Handle(new GetDietaryRestrictionsByUserIdQuery(new UserId(id)));
        return Ok(restrictions.Select(dr => dr.Restriction).ToArray());
    }
}
