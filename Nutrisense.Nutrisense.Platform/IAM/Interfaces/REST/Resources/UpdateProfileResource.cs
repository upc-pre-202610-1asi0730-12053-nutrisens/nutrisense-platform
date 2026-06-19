namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record UpdateProfileResource(
    string? FirstName,
    string? LastName,
    string? DateOfBirth,
    string? BiologicalSex,
    decimal? HeightCm,
    string? ActivityLevel,
    string? PreferredUnits,
    string? PreferredLanguage,
    string[]? MedicalConditions)
{
    /// <summary>User's first name. If null, the existing value is not changed.</summary>
    public string? FirstName { get; init; } = FirstName;

    /// <summary>User's last name. If null, the existing value is not changed.</summary>
    public string? LastName { get; init; } = LastName;

    /// <summary>User's date of birth. Format: ISO 8601 (yyyy-MM-dd). If null, the existing value is not changed.</summary>
    public string? DateOfBirth { get; init; } = DateOfBirth;

    /// <summary>Biological sex. Valid values: M (Male), F (Female), O (Other). If null, the existing value is not changed.</summary>
    public string? BiologicalSex { get; init; } = BiologicalSex;

    /// <summary>Height in centimeters. Decimal value allowed. If null, the existing value is not changed.</summary>
    public decimal? HeightCm { get; init; } = HeightCm;

    /// <summary>Activity level. Valid values: Sedentary, LightlyActive, ModeratelyActive, VeryActive, ExtremelyActive. If null, the existing value is not changed.</summary>
    public string? ActivityLevel { get; init; } = ActivityLevel;

    /// <summary>Preferred units for measurements. Valid values: Metric, Imperial. If null, the existing value is not changed.</summary>
    public string? PreferredUnits { get; init; } = PreferredUnits;

    /// <summary>Preferred language. Valid values: en, es. If null, the existing value is not changed.</summary>
    public string? PreferredLanguage { get; init; } = PreferredLanguage;

    /// <summary>Array of medical conditions or health issues. If null, the existing value is not changed.</summary>
    public string[]? MedicalConditions { get; init; } = MedicalConditions;
}
