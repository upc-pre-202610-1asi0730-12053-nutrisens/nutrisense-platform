namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.ValueObjects;

/// <summary>Value object representing the validated lifecycle state of a wearable connection: "connected", "disconnected" or "pending-auth".</summary>
public sealed record WearableStatus
{
    private static readonly HashSet<string> Valid = ["connected", "disconnected", "pending-auth"];

    /// <summary>The validated status string.</summary>
    public string Value { get; }

    /// <summary>Creates a validated wearable status, rejecting any value outside the allowed set.</summary>
    /// <param name="value">Candidate status; must be "connected", "disconnected" or "pending-auth".</param>
    public WearableStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Valid.Contains(value))
            throw new ArgumentException($"Invalid wearable status '{value}'. Must be one of: connected, disconnected, pending-auth.");
        Value = value;
    }
}
