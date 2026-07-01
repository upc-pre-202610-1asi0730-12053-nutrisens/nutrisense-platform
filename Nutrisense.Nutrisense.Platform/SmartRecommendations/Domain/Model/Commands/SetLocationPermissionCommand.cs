namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

public record SetLocationPermissionCommand(int UserId, bool Granted);
