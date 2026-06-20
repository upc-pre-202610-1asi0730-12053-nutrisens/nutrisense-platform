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
[Route("api/v1/pantry")]
[Tags("Pantry")]
[Authorize]
[Produces("application/json")]
public class PantryController(
    IRecsEngineCommandService commandService,
    IRecsEngineQueryService queryService) : ControllerBase
{
    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get user pantry", Description = "Retrieve pantry items for a user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var pantry = await queryService.Handle(new GetPantryByUserIdQuery(userId));
        return pantry is null ? Ok((object?)null) : Ok(PantryAssembler.ToResource(pantry));
    }

    [HttpPost("{userId:int}/items")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Register pantry items", Description = "Add multiple items to a user's pantry.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
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
                    StatusCode(StatusCodes.Status402PaymentRequired, new { error = error.ToString() }),
                _ => UnprocessableEntity(new { error = error.ToString() })
            });
    }

    [HttpDelete("{userId:int}/items/{itemId:int}")]
    [SwaggerOperation(Summary = "Remove pantry item", Description = "Remove an item from a user's pantry.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(int userId, int itemId)
    {
        var result = await commandService.Handle(new RemovePantryItemCommand(userId, itemId));
        return result.Fold(
            pantry => (IActionResult)Ok(PantryAssembler.ToResource(pantry)),
            error => error switch
            {
                Application.Errors.RemovePantryItemError.ItemNotFound => NotFound(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            });
    }

    [HttpPatch("{userId:int}/items/{id:int}")]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Update pantry item", Description = "Update quantity and unit for a pantry item.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(int userId, int id, [FromBody] UpdatePantryItemResource resource)
    {
        var result = await commandService.Handle(new UpdatePantryItemCommand(userId, id, resource.Quantity, resource.Unit));
        return result.Fold(
            pantry => (IActionResult)Ok(PantryAssembler.ToResource(pantry)),
            error => error switch
            {
                Application.Errors.UpdatePantryItemError.ItemNotFound => NotFound(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            });
    }
}
