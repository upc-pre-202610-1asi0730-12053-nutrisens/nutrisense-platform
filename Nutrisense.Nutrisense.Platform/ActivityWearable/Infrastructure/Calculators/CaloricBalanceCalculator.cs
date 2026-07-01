using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.Calculators;

/// <summary>Implementation of <see cref="ICaloricBalanceCalculator"/>. Sums activity calories and derives the caloric balance. (Balance currently returns active calories pending TDEE/intake integration.)</summary>
public class CaloricBalanceCalculator : ICaloricBalanceCalculator
{
    /// <summary>Sums the calories burned by the activities that fall on the given day.</summary>
    /// <param name="logs">Candidate activity logs; entries on other days are ignored.</param>
    /// <param name="date">Calendar day to total active calories for.</param>
    /// <returns>The active calories burned that day, in kilocalories.</returns>
    public decimal CalculateDailyActiveCalories(IEnumerable<ActivityLog> logs, DateOnly date) =>
        logs.Where(l => l.Date == date).Sum(l => l.CaloriesBurned);

    /// <summary>
    /// Computes the net caloric balance for a day as <c>intake − total expenditure</c>, where total
    /// expenditure is the TDEE baseline plus calories burned through explicitly logged activity:
    /// <c>balance = consumedCalories − (tdee + activeCalories)</c>. A negative result is a deficit
    /// (burned more than eaten), a positive result a surplus. Any component absent upstream is passed
    /// as 0, so the balance degrades gracefully to whatever data is available.
    /// </summary>
    /// <param name="tdee">Total daily energy expenditure baseline, in kilocalories.</param>
    /// <param name="activeCalories">Calories burned through logged activity, in kilocalories.</param>
    /// <param name="consumedCalories">Calories consumed through nutrition, in kilocalories.</param>
    /// <returns>The resulting net caloric balance, in kilocalories (negative = deficit, positive = surplus).</returns>
    public decimal CalculateBalance(decimal tdee, decimal activeCalories, decimal consumedCalories) =>
        consumedCalories - (tdee + activeCalories);
}
