namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;

/// <summary>
/// Domain service that estimates the energy expenditure of an activity when the provider did not
/// report it. Uses the MET (Metabolic Equivalent of Task) model:
/// <c>kcal = MET × weightKg × (durationMinutes / 60)</c>, where the MET is selected from the
/// activity kind and its effort level.
/// </summary>
public interface IActiveCalorieEstimator
{
    /// <summary>Estimates the calories burned by an activity, in kilocalories.</summary>
    /// <param name="activityType">Kind of activity (one of the supported domain activity kinds).</param>
    /// <param name="durationMinutes">Duration of the activity in minutes.</param>
    /// <param name="intensity">Effort level: "low", "medium" or "high".</param>
    /// <param name="weightKg">The user's body weight in kilograms, used to scale the estimate.</param>
    /// <returns>The estimated energy expenditure in kilocalories, rounded to whole kcal.</returns>
    decimal Estimate(string activityType, int durationMinutes, string intensity, decimal weightKg);
}
