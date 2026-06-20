using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Events;

public record WeatherContextRetrieved(int UserId, int CityId, string WeatherType) : DomainEventBase;
