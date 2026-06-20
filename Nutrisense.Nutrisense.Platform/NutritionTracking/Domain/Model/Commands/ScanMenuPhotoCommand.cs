namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

public record ScanMenuPhotoCommand(int UserId, string ImageBase64OrUri);
