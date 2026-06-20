namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

public record GenerateRecommendationCommand(int UserId, string Trigger);
