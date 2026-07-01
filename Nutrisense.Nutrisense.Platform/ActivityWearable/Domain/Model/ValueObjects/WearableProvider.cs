namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.ValueObjects;

/// <summary>Value object representing a validated wearable provider.</summary>
public sealed record WearableProvider
{
    private static readonly HashSet<string> Valid = ["google-health", "google-fit"];

    /// <summary>The validated provider string.</summary>
    public string Value { get; }

    /// <summary>Creates a validated wearable provider, rejecting any value outside the supported set.</summary>
    /// <param name="value">Candidate provider; must be "google-health" or "google-fit".</param>
    public WearableProvider(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Valid.Contains(value))
            throw new ArgumentException($"Invalid wearable provider '{value}'. Must be one of: google-health, google-fit.");
        Value = value;
    }
}
