using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

public class Recipe
{
    public int Id { get; private set; }
    public string Key { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string NameEs { get; private set; } = null!;
    public string GoalType { get; private set; } = null!;
    public int PrepTimeMinutes { get; private set; }
    public int Servings { get; private set; }
    public decimal TotalCalories { get; private set; }
    public decimal TotalProteinG { get; private set; }
    public decimal TotalCarbsG { get; private set; }
    public decimal TotalFatG { get; private set; }
    public decimal TotalFiberG { get; private set; }
    public List<string> RestrictionsConflict { get; private set; } = [];
    public List<RecipeIngredientItem> Ingredients { get; private set; } = [];

    protected Recipe() { }

    public Recipe(
        string key, string nameEn, string nameEs, string goalType,
        int prepTimeMinutes, int servings,
        decimal totalCalories, decimal totalProteinG, decimal totalCarbsG,
        decimal totalFatG, decimal totalFiberG,
        List<string> restrictionsConflict)
    {
        Key = key;
        NameEn = nameEn;
        NameEs = nameEs;
        GoalType = goalType;
        PrepTimeMinutes = prepTimeMinutes;
        Servings = servings;
        TotalCalories = totalCalories;
        TotalProteinG = totalProteinG;
        TotalCarbsG = totalCarbsG;
        TotalFatG = totalFatG;
        TotalFiberG = totalFiberG;
        RestrictionsConflict = restrictionsConflict;
    }

    /// <summary>Adds an ingredient line to this recipe. The FK is set by EF on persistence.</summary>
    public void AddIngredient(int ingredientCatalogItemId, decimal quantity, string unit) =>
        Ingredients.Add(new RecipeIngredientItem(ingredientCatalogItemId, quantity, unit));
}
