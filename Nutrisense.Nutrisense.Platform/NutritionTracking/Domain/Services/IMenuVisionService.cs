namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

public record MenuOption(string DishName, decimal? EstimatedCalories);

public record MenuRecognitionResult(bool Success, IEnumerable<MenuOption> Options);

public interface IMenuVisionService
{
    Task<MenuRecognitionResult> AnalyzeMenuAsync(string imageUri, CancellationToken ct);
}
