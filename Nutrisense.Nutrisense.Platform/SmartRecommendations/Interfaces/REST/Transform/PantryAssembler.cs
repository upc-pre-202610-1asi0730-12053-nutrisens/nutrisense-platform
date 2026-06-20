using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;

public static class PantryAssembler
{
    public static PantryResource ToResource(Pantry pantry) =>
        new(pantry.Id, pantry.UserId,
            pantry.Items.Select(i => new PantryItemResource(
                i.Id, i.IngredientCatalogItemId, i.Quantity, i.Unit, i.ExpiresAt)));
}
