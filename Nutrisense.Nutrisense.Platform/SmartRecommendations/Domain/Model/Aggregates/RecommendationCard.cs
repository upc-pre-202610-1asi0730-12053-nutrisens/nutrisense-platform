namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

public partial class RecommendationCard
{
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    public int? CityId { get; private set; }
    public string? WeatherType { get; private set; }
    public string? GoalType { get; private set; }
    public int? FoodId { get; private set; }
    public string FoodNameEn { get; private set; } = null!;
    public string FoodNameEs { get; private set; } = null!;
    public decimal EstimatedCalories { get; private set; }
    public decimal EstimatedProteinG { get; private set; }
    public decimal EstimatedCarbsG { get; private set; }
    public decimal EstimatedFatG { get; private set; }
    public string Badge { get; private set; } = null!;
    public string ContextLabelEn { get; private set; } = null!;
    public string ContextLabelEs { get; private set; } = null!;
    public List<string> RestrictionsConflict { get; private set; } = [];
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    protected RecommendationCard() { }

    public RecommendationCard(
        int userId,
        string foodNameEn,
        string foodNameEs,
        decimal calories,
        decimal protein,
        decimal carbs,
        decimal fat,
        string badge,
        string contextLabelEn,
        string contextLabelEs,
        int? foodId,
        int? cityId,
        string? weatherType,
        string? goalType,
        IReadOnlyList<string>? restrictionsConflict = null)
    {
        UserId = userId;
        FoodNameEn = foodNameEn;
        FoodNameEs = foodNameEs;
        EstimatedCalories = calories;
        EstimatedProteinG = protein;
        EstimatedCarbsG = carbs;
        EstimatedFatG = fat;
        Badge = badge;
        ContextLabelEn = contextLabelEn;
        ContextLabelEs = contextLabelEs;
        FoodId = foodId;
        CityId = cityId;
        WeatherType = weatherType;
        GoalType = goalType;
        RestrictionsConflict = restrictionsConflict?.ToList() ?? [];
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate() => IsActive = false;
}
