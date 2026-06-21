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
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/pantry")]
[Tags("Pantry")]
[Authorize]
[Produces("application/json")]
public class PantryController(
    IRecsEngineCommandService commandService,
    IRecsEngineQueryService queryService,
    IStringLocalizer<SmartRecommendationsMessages> localizer) : ControllerBase
{
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get user pantry", Description = "Retrieve pantry items for a user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Pantry retrieved successfully (body is null when the user has no pantry yet).", typeof(PantryResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var pantry = await queryService.Handle(new GetPantryByUserIdQuery(userId));
        return pantry is null ? Ok((object?)null) : Ok(PantryAssembler.ToResource(pantry));
    }

    [HttpPost("{userId:int}/items")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Register pantry items", Description = "Add multiple items to a user's pantry.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Pantry items registered successfully.", typeof(PantryResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status402PaymentRequired, "Your current plan does not allow adding more pantry items.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "The pantry items could not be registered.", typeof(ErrorResponse))]
    public async Task<IActionResult> RegisterItems(int userId, [FromBody] RegisterPantryItemsResource resource)
    {
        var items = resource.Items
            .Select(i => new PantryItemInput(i.IngredientCatalogItemId, i.Quantity, i.Unit))
            .ToArray();
        var result = await commandService.Handle(new RegisterPantryItemsCommand(userId, items));
        return result.Fold(
            pantry => (IActionResult)Ok(PantryAssembler.ToResource(pantry)),
            error => error switch
            {
                Application.Errors.RegisterPantryItemsError.PlanNotSufficient =>
                    StatusCode(StatusCodes.Status402PaymentRequired, new ErrorResponse(localizer["PlanDoesNotAllowMorePantryItems"].Value)),
                _ => UnprocessableEntity(new ErrorResponse(localizer["PantryItemsRegistrationFailed"].Value))
            });
    }

    [HttpDelete("{userId:int}/items/{itemId:int}")]
    [SwaggerOperation(Summary = "Remove pantry item", Description = "Remove an item from a user's pantry.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Pantry item removed successfully. Returns the updated pantry.", typeof(PantryResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested pantry item was not found.", typeof(ErrorResponse))]
    public async Task<IActionResult> RemoveItem(int userId, int itemId)
    {
        var result = await commandService.Handle(new RemovePantryItemCommand(userId, itemId));
        return result.Fold(
            pantry => (IActionResult)Ok(PantryAssembler.ToResource(pantry)),
            error => error switch
            {
                Application.Errors.RemovePantryItemError.ItemNotFound => NotFound(new ErrorResponse(localizer["PantryItemNotFound"].Value)),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            });
    }

    [HttpPatch("{userId:int}/items/{id:int}")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Update pantry item", Description = "Update quantity and unit for a pantry item.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Pantry item updated successfully. Returns the updated pantry.", typeof(PantryResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested pantry item was not found.", typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateItem(int userId, int id, [FromBody] UpdatePantryItemResource resource)
    {
        var result = await commandService.Handle(new UpdatePantryItemCommand(userId, id, resource.Quantity, resource.Unit));
        return result.Fold(
            pantry => (IActionResult)Ok(PantryAssembler.ToResource(pantry)),
            error => error switch
            {
                Application.Errors.UpdatePantryItemError.ItemNotFound => NotFound(new ErrorResponse(localizer["PantryItemNotFound"].Value)),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            });
    }
}
