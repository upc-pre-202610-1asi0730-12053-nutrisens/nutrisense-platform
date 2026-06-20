namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

public record DeleteNutritionLogEntryCommand(int EntryId, int UserId);
