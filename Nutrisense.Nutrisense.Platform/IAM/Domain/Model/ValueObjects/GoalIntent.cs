namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record GoalIntent
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "weight-loss", "muscle-gain"
    };

    public string Value { get; }

    public GoalIntent(string value)
    {
        if (!Allowed.Contains(value))
            throw new ArgumentException(
                $"'{value}' is not a valid goal intent. Allowed: weight-loss, muscle-gain.",
                nameof(value));
        Value = value.ToLowerInvariant();
    }
}
