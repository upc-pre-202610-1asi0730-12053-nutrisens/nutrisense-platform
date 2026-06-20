namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;

public sealed record MealType
{
    private static readonly HashSet<string> Valid = ["breakfast", "lunch", "snack", "dinner"];

    public string Value { get; }

    public MealType(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Valid.Contains(value.ToLowerInvariant()))
            throw new ArgumentException($"Invalid meal type '{value}'. Must be one of: breakfast, lunch, snack, dinner.");
        Value = value.ToLowerInvariant();
    }
}
