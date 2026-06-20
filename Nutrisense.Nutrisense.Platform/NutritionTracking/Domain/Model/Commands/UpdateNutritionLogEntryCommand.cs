namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

public record UpdateNutritionLogEntryCommand(int EntryId, int UserId, decimal QuantityG);
