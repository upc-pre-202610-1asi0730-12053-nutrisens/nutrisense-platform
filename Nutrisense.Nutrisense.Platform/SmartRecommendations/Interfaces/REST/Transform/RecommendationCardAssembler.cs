using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;

public static class RecommendationCardAssembler
{
    public static RecommendationCardResource ToResource(RecommendationCard card) =>
        new(card.Id, card.UserId, card.FoodId, card.FoodNameEn, card.FoodNameEs,
            card.EstimatedCalories, card.EstimatedProteinG, card.EstimatedCarbsG, card.EstimatedFatG,
            card.Badge, card.ContextLabelEn, card.ContextLabelEs,
            card.RestrictionsConflict, card.IsActive, card.CreatedAt);
}
