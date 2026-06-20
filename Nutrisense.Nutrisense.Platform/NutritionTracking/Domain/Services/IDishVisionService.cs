namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

/// <summary>A single food item detected on a plate, with the model's estimated weight in grams.</summary>
public record DetectedDishItem(string Name, decimal EstimatedQuantityG);

/// <summary>
/// Result of recognizing a plate photo: whether the recognition succeeded and the list of
/// detected items. An empty list with <c>Success == true</c> means "nothing recognized"
/// (the caller should surface a fallback), as opposed to a recognition failure.
/// </summary>
public record DishRecognitionResult(bool Success, IReadOnlyList<DetectedDishItem> Items);

public interface IDishVisionService
{
    Task<DishRecognitionResult> RecognizeDishAsync(string imageBase64OrUri, CancellationToken ct);
}
