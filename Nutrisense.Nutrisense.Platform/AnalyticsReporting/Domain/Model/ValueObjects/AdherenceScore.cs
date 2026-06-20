namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.ValueObjects;

/// <summary>Value object expressing how closely a user met their nutrition goals, normalized to 0–1.</summary>
public sealed record AdherenceScore
{
    public decimal Value { get; }

    public AdherenceScore(decimal value)
    {
        if (value < 0 || value > 1)
            throw new ArgumentException($"AdherenceScore must be between 0 and 1, got {value}.");
        Value = value;
    }
}
