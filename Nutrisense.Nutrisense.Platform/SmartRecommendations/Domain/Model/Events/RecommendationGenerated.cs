using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Events;

public record RecommendationGenerated(int UserId, int RecommendationCardId, string ContextLabel) : DomainEventBase;
