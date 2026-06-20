namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record NutritionLogResource(
    int Id,
    int UserId,
    int FoodId,
    string FoodNameEn,
    string FoodNameEs,
    string MealType,
    string Date,
    decimal QuantityG,
    decimal Calories,
    decimal ProteinG,
    decimal CarbsG,
    decimal FatG,
    decimal FiberG,
    decimal SugarG,
    string Source,
    DateTimeOffset LoggedAt,
    string? ScanType,
    decimal? ScanConfidence,
    string? ScanImageUri)
{
    /// <summary>Unique identifier of the nutrition log entry.</summary>
    public int Id { get; } = Id;

    /// <summary>User ID of the person who logged the meal.</summary>
    public int UserId { get; } = UserId;

    /// <summary>Food ID from the food catalog.</summary>
    public int FoodId { get; } = FoodId;

    /// <summary>Food name in English.</summary>
    public string FoodNameEn { get; } = FoodNameEn;

    /// <summary>Food name in Spanish.</summary>
    public string FoodNameEs { get; } = FoodNameEs;

    /// <summary>Type of meal. Valid values: breakfast, lunch, dinner, snack.</summary>
    public string MealType { get; } = MealType;

    /// <summary>Date of the meal in ISO 8601 format (yyyy-MM-dd).</summary>
    public string Date { get; } = Date;

    /// <summary>Quantity of food consumed in grams (g).</summary>
    public decimal QuantityG { get; } = QuantityG;

    /// <summary>Total calories consumed in this meal (kcal).</summary>
    public decimal Calories { get; } = Calories;

    /// <summary>Total protein consumed in this meal (g).</summary>
    public decimal ProteinG { get; } = ProteinG;

    /// <summary>Total carbohydrates consumed in this meal (g).</summary>
    public decimal CarbsG { get; } = CarbsG;

    /// <summary>Total fat consumed in this meal (g).</summary>
    public decimal FatG { get; } = FatG;

    /// <summary>Total dietary fiber consumed in this meal (g).</summary>
    public decimal FiberG { get; } = FiberG;

    /// <summary>Total sugar consumed in this meal (g).</summary>
    public decimal SugarG { get; } = SugarG;

    /// <summary>Source of the log entry (e.g., manual, scan, app). Valid values: manual, scan, app.</summary>
    public string Source { get; } = Source;

    /// <summary>Timestamp when the entry was logged in ISO 8601 format (UTC).</summary>
    public DateTimeOffset LoggedAt { get; } = LoggedAt;

    /// <summary>Type of scan if entry was created from a scan (e.g., dish, menu). Null if entry was logged manually.</summary>
    public string? ScanType { get; } = ScanType;

    /// <summary>Confidence score of the scan detection (0-1). Null if entry was logged manually.</summary>
    public decimal? ScanConfidence { get; } = ScanConfidence;

    /// <summary>URI or base64-encoded image data from the scan. Null if entry was logged manually.</summary>
    public string? ScanImageUri { get; } = ScanImageUri;
}
