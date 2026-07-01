using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.Calculators;

/// <summary>
/// MET-based implementation of <see cref="IActiveCalorieEstimator"/>. Looks up a representative
/// MET value for the activity kind and effort level, then applies
/// <c>kcal = MET × weightKg × (durationMinutes / 60)</c>. Activity kinds and intensities outside
/// the known tables fall back to neutral defaults so an estimate is always produced.
/// </summary>
public class MetActiveCalorieEstimator : IActiveCalorieEstimator
{
    /// <summary>MET values per supported activity kind, indexed by effort level (low, medium, high).</summary>
    private static readonly IReadOnlyDictionary<string, (double Low, double Medium, double High)> MetByActivity =
        new Dictionary<string, (double, double, double)>(StringComparer.OrdinalIgnoreCase)
        {
            ["walking"] = (2.8, 3.5, 4.3),
            ["running"] = (7.0, 9.8, 11.5),
            ["cycling"] = (4.0, 6.8, 10.0),
            ["swimming"] = (4.5, 7.0, 9.5),
            ["rowing"] = (4.0, 7.0, 8.5),
            ["elliptical"] = (4.6, 5.0, 7.0),
            ["hiit"] = (6.0, 8.0, 10.0),
            ["strength-training"] = (3.5, 5.0, 6.0),
            ["yoga"] = (2.0, 2.5, 4.0),
            ["pilates"] = (2.3, 3.0, 4.0),
            ["boxing"] = (5.5, 7.8, 10.0),
            ["dance"] = (3.5, 5.0, 7.3),
            ["cardio"] = (4.0, 6.0, 8.0),
        };

    /// <summary>MET used when the activity kind is unknown (mirrors the provider's "cardio" fallback).</summary>
    private static readonly (double Low, double Medium, double High) DefaultMet = (3.0, 4.0, 6.0);

    /// <inheritdoc />
    public decimal Estimate(string activityType, int durationMinutes, string intensity, decimal weightKg)
    {
        if (durationMinutes <= 0 || weightKg <= 0)
            return 0m;

        var met = MetByActivity.TryGetValue(activityType ?? string.Empty, out var row) ? row : DefaultMet;
        var value = intensity?.ToLowerInvariant() switch
        {
            "low" => met.Low,
            "high" => met.High,
            _ => met.Medium
        };

        var kcal = (decimal)value * weightKg * (durationMinutes / 60m);
        return Math.Round(kcal, MidpointRounding.AwayFromZero);
    }
}
