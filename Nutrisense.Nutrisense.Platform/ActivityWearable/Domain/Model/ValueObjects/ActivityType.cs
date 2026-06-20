namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.ValueObjects;

/// <summary>Value object representing a validated activity kind, constrained to a non-empty string of at most 50 characters.</summary>
public sealed record ActivityType
{
    /// <summary>The validated activity type string.</summary>
    public string Value { get; }

    /// <summary>Creates a validated activity type, rejecting empty values or values longer than 50 characters.</summary>
    /// <param name="value">Candidate activity type.</param>
    public ActivityType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Activity type cannot be empty.");
        if (value.Length > 50)
            throw new ArgumentException("Activity type cannot exceed 50 characters.");
        Value = value;
    }
}
