namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

/// <summary>
/// Generates and imports recipe suggestions. When <paramref name="GoalTypes"/> is null/empty,
/// recipes are generated for every supported goal.
/// </summary>
public record ImportRecipeSuggestionsCommand(
    IReadOnlyList<string>? GoalTypes,
    int MaxPerGoal);
