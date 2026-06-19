namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

/// <summary>Validated weight value in kilograms, enforcing a range of 20–500 kg.</summary>
public sealed record WeightKg
{
    public decimal Value { get; }

    public WeightKg(decimal value)
    {
        if (value < 20m || value > 500m)
            throw new ArgumentException("Weight must be between 20 and 500 kg.", nameof(value));
        Value = value;
    }
}
