namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Entities;

/// <summary>Entity capturing a user's nutrition totals and adherence for a single day.</summary>
public class ProgressSnapshot
{
    public int Id { get; private set; }
    public int UserAnalyticsId { get; private set; }
    public DateOnly Date { get; private set; }
    public decimal TotalCalories { get; private set; }
    public decimal TotalProteinG { get; private set; }
    public decimal TotalCarbsG { get; private set; }
    public decimal TotalFatG { get; private set; }
    public decimal AdherenceScore { get; private set; }

    /// <summary>True when the day met the streak criteria (all required meals + calorie/protein thresholds).</summary>
    public bool IsComplete { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    protected ProgressSnapshot() { }

    public ProgressSnapshot(DateOnly date, decimal totalCalories, decimal totalProteinG,
        decimal totalCarbsG, decimal totalFatG, decimal adherenceScore, bool isComplete)
    {
        Date = date;
        TotalCalories = totalCalories;
        TotalProteinG = totalProteinG;
        TotalCarbsG = totalCarbsG;
        TotalFatG = totalFatG;
        AdherenceScore = adherenceScore;
        IsComplete = isComplete;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
