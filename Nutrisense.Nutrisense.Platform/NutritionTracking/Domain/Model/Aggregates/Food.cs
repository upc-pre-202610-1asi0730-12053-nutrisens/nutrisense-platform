using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;

public partial class Food
{
    public int Id { get; private set; }
    public string Key { get; private set; } = null!;
    public string Source { get; private set; } = null!;
    public string? ExternalId { get; private set; }
    public string NameEn { get; private set; } = null!;
    public string NameEs { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public decimal ServingSizeG { get; private set; }
    public string ServingUnit { get; private set; } = null!;
    public decimal CaloriesPer100g { get; private set; }
    public decimal ProteinPer100g { get; private set; }
    public decimal CarbsPer100g { get; private set; }
    public decimal FatPer100g { get; private set; }
    public decimal FiberPer100g { get; private set; }
    public decimal SugarPer100g { get; private set; }
    public List<string> Restrictions { get; private set; } = new();

    protected Food() { }

    public Food(RegisterFoodCommand command)
    {
        Source = new FoodSource(command.Source).Value;
        Key = $"{Source}-{command.NameEn.Trim().ToLowerInvariant().Replace(" ", "-")}";
        ExternalId = command.ExternalId;
        NameEn = command.NameEn.Trim();
        NameEs = command.NameEs.Trim();
        Category = string.IsNullOrWhiteSpace(command.Category) ? "Other" : command.Category.Trim();
        ServingSizeG = command.ServingSizeG;
        ServingUnit = new ServingUnit(command.ServingUnit).Value;
        CaloriesPer100g = command.CaloriesPer100g;
        ProteinPer100g = command.ProteinPer100g;
        CarbsPer100g = command.CarbsPer100g;
        FatPer100g = command.FatPer100g;
        FiberPer100g = command.FiberPer100g;
        SugarPer100g = command.SugarPer100g;
        Restrictions = command.Restrictions.ToList();
    }
}
