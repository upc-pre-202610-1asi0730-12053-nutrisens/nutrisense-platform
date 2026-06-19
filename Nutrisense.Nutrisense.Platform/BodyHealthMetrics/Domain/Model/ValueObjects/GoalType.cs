namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

/// <summary>Constrained goal-type value object accepting "weight-loss" or "muscle-gain".</summary>
public sealed record GoalType
{
    private static readonly HashSet<string> ValidValues = ["weight-loss", "muscle-gain"];

    public string Value { get; }

    public GoalType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Goal type cannot be empty.", nameof(value));
        var lower = value.Trim().ToLowerInvariant();
        if (!ValidValues.Contains(lower))
            throw new ArgumentException($"Goal type must be one of: {string.Join(", ", ValidValues)}.", nameof(value));
        Value = lower;
    }
}
