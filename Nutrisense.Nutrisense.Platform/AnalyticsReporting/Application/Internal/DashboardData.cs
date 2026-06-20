namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal;

/// <summary>Read model aggregating a user's nutrition, activity, and adherence figures for one day.</summary>
public record DashboardData(
    int UserId,
    DateOnly Date,
    decimal TotalCalories,
    decimal TotalProteinG,
    decimal TotalCarbsG,
    decimal TotalFatG,
    decimal ActiveCaloriesBurned,
    decimal? AdherenceScore,
    int CurrentStreak,
    decimal WeeklyCompletionRate);
