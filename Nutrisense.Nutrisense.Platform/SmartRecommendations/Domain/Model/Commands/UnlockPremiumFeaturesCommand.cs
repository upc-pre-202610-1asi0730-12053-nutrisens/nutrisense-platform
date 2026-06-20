namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

public record UnlockPremiumFeaturesCommand(int UserId, string PlanKey);
