namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Resources;

/// <summary>API response exposing a user's dashboard figures for a day.</summary>
public record DashboardResource(
    /// <summary>The user's unique identifier.</summary>
    int UserId,
    /// <summary>The date for which dashboard metrics are reported in ISO 8601 format (yyyy-MM-dd).</summary>
    string Date,
    /// <summary>Total calories consumed on the specified date in kilocalories (kcal).</summary>
    decimal TotalCalories,
    /// <summary>Total protein consumed in grams (g).</summary>
    decimal TotalProteinG,
    /// <summary>Total carbohydrates consumed in grams (g).</summary>
    decimal TotalCarbsG,
    /// <summary>Total fat consumed in grams (g).</summary>
    decimal TotalFatG,
    /// <summary>Active calories burned through physical activity in kilocalories (kcal).</summary>
    decimal ActiveCaloriesBurned,
    /// <summary>The user's adherence score as a percentage (0-100), or null if not calculable.</summary>
    decimal? AdherenceScore,
    /// <summary>The number of consecutive days the user has viewed the dashboard.</summary>
    int CurrentStreak,
    /// <summary>The percentage of days the user completed their goals in the current week (0-1).</summary>
    decimal WeeklyCompletionRate);
