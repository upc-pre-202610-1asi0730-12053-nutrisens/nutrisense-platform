namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;

public record GetRecipesByGoalQuery(string GoalType, int? MaxPrepMinutes);
