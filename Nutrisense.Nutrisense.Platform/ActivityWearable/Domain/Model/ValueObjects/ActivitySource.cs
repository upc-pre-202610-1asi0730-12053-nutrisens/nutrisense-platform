namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.ValueObjects;

/// <summary>Value object representing the validated origin of an activity log: either "manual" or "google-fit".</summary>
public sealed record ActivitySource
{
    private static readonly HashSet<string> Valid = ["manual", "google-fit"];

    /// <summary>The validated source string.</summary>
    public string Value { get; }

    /// <summary>Creates a validated activity source, rejecting any value outside the allowed set.</summary>
    /// <param name="value">Candidate source; must be "manual" or "google-fit".</param>
    public ActivitySource(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Valid.Contains(value))
            throw new ArgumentException($"Invalid activity source '{value}'. Must be one of: manual, google-fit.");
        Value = value;
    }
}
