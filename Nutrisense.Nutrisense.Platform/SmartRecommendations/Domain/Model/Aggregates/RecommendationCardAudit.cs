using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

public partial class RecommendationCard : IAuditableEntity
{
    DateTimeOffset? IAuditableEntity.CreatedAt { get; set; }
    DateTimeOffset? IAuditableEntity.UpdatedAt { get; set; }
}
