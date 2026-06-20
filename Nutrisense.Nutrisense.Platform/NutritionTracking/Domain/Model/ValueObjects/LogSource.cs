namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;

public sealed record LogSource
{
    private static readonly HashSet<string> Valid = ["manual", "scan-dish", "scan-menu"];

    public string Value { get; }

    public LogSource(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Valid.Contains(value.ToLowerInvariant()))
            throw new ArgumentException($"Invalid log source '{value}'. Must be one of: manual, scan-dish, scan-menu.");
        Value = value.ToLowerInvariant();
    }
}
