using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Events;

public record LocationDetected(int UserId, int CityId) : DomainEventBase;
