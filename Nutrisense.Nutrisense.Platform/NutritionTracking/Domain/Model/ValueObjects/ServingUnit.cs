namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;

public sealed record ServingUnit
{
    private static readonly HashSet<string> Valid = ["g", "ml", "unit"];

    public string Value { get; }

    public ServingUnit(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Valid.Contains(value.ToLowerInvariant()))
            throw new ArgumentException($"Invalid serving unit '{value}'. Must be one of: g, ml, unit.");
        Value = value.ToLowerInvariant();
    }
}
