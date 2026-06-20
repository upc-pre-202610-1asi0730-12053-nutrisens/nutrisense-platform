namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;

public sealed record FoodSource
{
    private static readonly HashSet<string> Valid = ["usda", "manual", "open-food-facts", "ai-dish-scan", "ai-menu-scan", "ai-local-rec"];

    public string Value { get; }

    public FoodSource(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Valid.Contains(value.ToLowerInvariant()))
            throw new ArgumentException($"Invalid food source '{value}'. Must be one of: usda, manual, open-food-facts, ai-dish-scan, ai-menu-scan, ai-local-rec.");
        Value = value.ToLowerInvariant();
    }
}
