namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record UserResource(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string? DateOfBirth,
    string? BiologicalSex,
    decimal? HeightCm,
    string? Goal,
    string? ActivityLevel,
    string PreferredUnits,
    string PreferredLanguage,
    string[] MedicalConditions,
    string[] DietaryRestrictions)
{
    /// <summary>Unique identifier for the user.</summary>
    public int Id { get; init; } = Id;

    /// <summary>User's email address.</summary>
    public string Email { get; init; } = Email;

    /// <summary>User's first name.</summary>
    public string FirstName { get; init; } = FirstName;

    /// <summary>User's last name.</summary>
    public string LastName { get; init; } = LastName;

    /// <summary>User's date of birth. Format: ISO 8601 (yyyy-MM-dd).</summary>
    public string? DateOfBirth { get; init; } = DateOfBirth;

    /// <summary>Biological sex. Valid values: M (Male), F (Female), O (Other).</summary>
    public string? BiologicalSex { get; init; } = BiologicalSex;

    /// <summary>Height in centimeters.</summary>
    public decimal? HeightCm { get; init; } = HeightCm;

    /// <summary>Health goal intent. Valid values: Bulk, Cut, Maintain, Lose.</summary>
    public string? Goal { get; init; } = Goal;

    /// <summary>Activity level. Valid values: Sedentary, LightlyActive, ModeratelyActive, VeryActive, ExtremelyActive.</summary>
    public string? ActivityLevel { get; init; } = ActivityLevel;

    /// <summary>Preferred units for measurements. Valid values: Metric, Imperial.</summary>
    public string PreferredUnits { get; init; } = PreferredUnits;

    /// <summary>Preferred language. Valid values: en, es.</summary>
    public string PreferredLanguage { get; init; } = PreferredLanguage;

    /// <summary>Array of medical conditions or health issues.</summary>
    public string[] MedicalConditions { get; init; } = MedicalConditions;

    /// <summary>Array of dietary restrictions. Valid values: Vegan, Vegetarian, GlutenFree, Halal, Kosher, Keto, Paleo, DairyFree, NutFree, SoyFree.</summary>
    public string[] DietaryRestrictions { get; init; } = DietaryRestrictions;
}
