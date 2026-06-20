namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

public record UpdatePantryItemCommand(int UserId, int PantryItemId, decimal Quantity, string Unit);
