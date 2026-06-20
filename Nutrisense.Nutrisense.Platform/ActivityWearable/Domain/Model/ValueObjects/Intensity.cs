namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.ValueObjects;

/// <summary>Value object representing the validated effort level of an activity, normalized to lowercase: "low", "medium" or "high".</summary>
public sealed record Intensity
{
    private static readonly HashSet<string> Valid = ["low", "medium", "high"];

    /// <summary>The validated, lowercased intensity string.</summary>
    public string Value { get; }

    /// <summary>Creates a validated intensity, normalizing to lowercase and rejecting any value outside the allowed set.</summary>
    /// <param name="value">Candidate intensity; must be "low", "medium" or "high" (case-insensitive).</param>
    public Intensity(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Valid.Contains(value.ToLowerInvariant()))
            throw new ArgumentException($"Invalid intensity '{value}'. Must be one of: low, medium, high.");
        Value = value.ToLowerInvariant();
    }
}
