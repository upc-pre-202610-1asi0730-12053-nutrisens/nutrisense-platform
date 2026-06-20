namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

/// <summary>Immutable BMI result carrying a numeric value and its WHO weight-status category.</summary>
public sealed record BmiResult
{
    private static readonly HashSet<string> ValidCategories = ["underweight", "normal", "overweight", "obese"];

    public decimal Value { get; }
    public string Category { get; }

    public BmiResult(decimal value, string category)
    {
        if (value <= 0m)
            throw new ArgumentException("BMI value must be positive.", nameof(value));
        if (!ValidCategories.Contains(category))
            throw new ArgumentException($"BMI category must be one of: {string.Join(", ", ValidCategories)}.", nameof(category));
        Value = value;
        Category = category;
    }
}
