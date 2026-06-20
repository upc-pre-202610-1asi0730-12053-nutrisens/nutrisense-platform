namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

public record PantryItemInput(int IngredientCatalogItemId, decimal Quantity, string Unit);

public record RegisterPantryItemsCommand(int UserId, PantryItemInput[] Items);
