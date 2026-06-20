namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

public record RegisterFoodCommand(
    string NameEn,
    string NameEs,
    string Category,
    string Source,
    string? ExternalId,
    decimal ServingSizeG,
    string ServingUnit,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    decimal FiberPer100g,
    decimal SugarPer100g,
    string[] Restrictions);
