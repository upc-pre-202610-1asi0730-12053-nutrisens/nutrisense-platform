namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;

public record SearchCitiesQuery(string Query, int Limit = 5);
