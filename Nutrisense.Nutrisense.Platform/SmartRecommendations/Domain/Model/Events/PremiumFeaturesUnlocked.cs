using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Events;

public record PremiumFeaturesUnlocked(int UserId, string PlanKey) : DomainEventBase;
