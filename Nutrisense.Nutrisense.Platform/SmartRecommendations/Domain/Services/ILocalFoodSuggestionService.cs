namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

/// <summary>A single food/dish the generator suggests for a location + weather + goal context.</summary>
public record LocalFoodSuggestion(string NameEn, string NameEs, string? Note);

/// <summary>
/// Generates names of foods/dishes that are commonly available in a given city and appropriate for the
/// current weather and the user's goal. Returns NAMES only — turning them into catalog foods (with macros)
/// is the food-provisioning step's job. Implementations MUST degrade gracefully (return an empty list)
/// on any failure so recommendation generation never breaks on an LLM outage.
/// </summary>
public interface ILocalFoodSuggestionService
{
    Task<IReadOnlyList<LocalFoodSuggestion>> SuggestAsync(
        string cityName,
        string country,
        string weatherCondition,
        decimal tempC,
        string weatherType,
        string goalType,
        IReadOnlyList<string> restrictions,
        int count,
        CancellationToken ct = default);
}
