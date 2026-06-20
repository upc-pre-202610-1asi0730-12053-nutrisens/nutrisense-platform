namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

/// <summary>Validated weekly weight-change pace in kg/week, bounded between −2.0 and +2.0.</summary>
public sealed record WeeklyRate
{
    public decimal Value { get; }

    public WeeklyRate(decimal value)
    {
        if (value < -2.0m || value > 2.0m)
            throw new ArgumentException("Weekly rate must be between -2.0 and 2.0 kg/week.", nameof(value));
        Value = value;
    }
}
