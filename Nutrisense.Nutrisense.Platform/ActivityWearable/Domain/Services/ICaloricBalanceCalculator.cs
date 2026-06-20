using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;

/// <summary>Domain service that derives active-calorie totals and the overall caloric balance from activity data.</summary>
public interface ICaloricBalanceCalculator
{
    /// <summary>Sums the calories burned by the activities that fall on a specific day.</summary>
    /// <param name="logs">Candidate activity logs; entries on other days are ignored.</param>
    /// <param name="date">Calendar day to total active calories for.</param>
    /// <returns>The active calories burned that day, in kilocalories.</returns>
    decimal CalculateDailyActiveCalories(IEnumerable<ActivityLog> logs, DateOnly date);

    /// <summary>Computes the net caloric balance from energy expenditure and intake.</summary>
    /// <param name="tdee">Total daily energy expenditure baseline, in kilocalories.</param>
    /// <param name="activeCalories">Calories burned through logged activity, in kilocalories.</param>
    /// <param name="consumedCalories">Calories consumed through nutrition, in kilocalories.</param>
    /// <returns>The resulting caloric balance, in kilocalories.</returns>
    decimal CalculateBalance(decimal tdee, decimal activeCalories, decimal consumedCalories);
}
