using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;

public static class IngredientCatalogAssembler
{
    public static IngredientCatalogItemResource ToResource(IngredientCatalogItem item) =>
        new(item.Id, item.Key, item.NameEn, item.NameEs,
            item.FoodId, item.Category, item.DefaultUnit, item.GramsPerUnit);
}
