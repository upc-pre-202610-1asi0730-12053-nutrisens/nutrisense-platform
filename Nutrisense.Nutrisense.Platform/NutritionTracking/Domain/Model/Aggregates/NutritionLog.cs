using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;

public partial class NutritionLog
{
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    public int FoodId { get; private set; }
    public string FoodNameEn { get; private set; } = null!;
    public string FoodNameEs { get; private set; } = null!;
    public string MealType { get; private set; } = null!;
    public DateOnly Date { get; private set; }
    public decimal QuantityG { get; private set; }
    public decimal Calories { get; private set; }
    public decimal ProteinG { get; private set; }
    public decimal CarbsG { get; private set; }
    public decimal FatG { get; private set; }
    public decimal FiberG { get; private set; }
    public decimal SugarG { get; private set; }
    public string Source { get; private set; } = null!;
    public DateTimeOffset LoggedAt { get; private set; }
    public string? ScanType { get; private set; }
    public decimal? ScanConfidence { get; private set; }
    public string? ScanImageUri { get; private set; }

    protected NutritionLog() { }

    public NutritionLog(LogMealToDailyLogCommand command, Food food)
    {
        UserId = command.UserId;
        FoodId = food.Id;
        FoodNameEn = food.NameEn;
        FoodNameEs = food.NameEs;
        MealType = new MealType(command.MealType).Value;
        Date = command.Date;
        QuantityG = command.QuantityG;
        Source = new LogSource(command.Source).Value;
        LoggedAt = DateTimeOffset.UtcNow;
        ApplyMacros(food, command.QuantityG);
    }

    public NutritionLog(ConfirmScanResultCommand command, Food food, decimal confidence, string imageUri)
    {
        UserId = command.UserId;
        FoodId = food.Id;
        FoodNameEn = food.NameEn;
        FoodNameEs = food.NameEs;
        MealType = new MealType(command.MealType).Value;
        Date = command.Date;
        QuantityG = command.QuantityG;
        Source = "smart-scan";
        LoggedAt = DateTimeOffset.UtcNow;
        ScanType = "dish";
        ScanConfidence = confidence;
        ScanImageUri = imageUri;
        ApplyMacros(food, command.QuantityG);
    }

    public NutritionLog(SelectMenuOptionCommand command, Food food)
    {
        UserId = command.UserId;
        FoodId = food.Id;
        FoodNameEn = food.NameEn;
        FoodNameEs = food.NameEs;
        MealType = new MealType(command.MealType).Value;
        Date = command.Date;
        QuantityG = command.QuantityG;
        Source = "manual";
        LoggedAt = DateTimeOffset.UtcNow;
        ScanType = "menu";
        ApplyMacros(food, command.QuantityG);
    }

    public void UpdateQuantity(Food food, decimal newQuantityG)
    {
        if (newQuantityG <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(newQuantityG));
        QuantityG = newQuantityG;
        ApplyMacros(food, newQuantityG);
    }

    private void ApplyMacros(Food food, decimal quantityG)
    {
        var factor = quantityG / 100;
        Calories = Math.Round(food.CaloriesPer100g * factor, 2);
        ProteinG = Math.Round(food.ProteinPer100g * factor, 2);
        CarbsG = Math.Round(food.CarbsPer100g * factor, 2);
        FatG = Math.Round(food.FatPer100g * factor, 2);
        FiberG = Math.Round(food.FiberPer100g * factor, 2);
        SugarG = Math.Round(food.SugarPer100g * factor, 2);
    }
}
