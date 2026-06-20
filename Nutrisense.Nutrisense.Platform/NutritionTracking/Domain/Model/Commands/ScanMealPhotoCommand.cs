namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

public record ScanMealPhotoCommand(int UserId, string ImageBase64OrUri);
