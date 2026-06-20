namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record ScanPhotoResource(int UserId, string ImageBase64OrUri)
{
    /// <summary>User ID of the person scanning the photo.</summary>
    public int UserId { get; } = UserId;

    /// <summary>Image data encoded in base64 format or a URI pointing to the image.</summary>
    public string ImageBase64OrUri { get; } = ImageBase64OrUri;
}
