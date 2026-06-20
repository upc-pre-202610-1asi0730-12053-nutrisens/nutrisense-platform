namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

public record DetectLocationCommand(int UserId, decimal Lat, decimal Lng);
