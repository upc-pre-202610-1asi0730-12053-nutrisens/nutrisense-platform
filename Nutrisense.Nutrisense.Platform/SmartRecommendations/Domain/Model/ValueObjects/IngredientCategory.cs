namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.ValueObjects;

public sealed record IngredientCategory(string Value)
{
    private static readonly string[] Allowed =
        ["protein", "grain", "vegetable", "fruit", "fat", "dairy", "legume", "supplement"];

    public string Value { get; } = Allowed.Contains(Value)
        ? Value
        : throw new ArgumentException($"Invalid ingredient category: {Value}");
}
