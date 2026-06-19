using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;

public record UpdateProfileCommand(
    UserId UserId,
    string? FirstName,
    string? LastName,
    DateOnly? DateOfBirth,
    string? BiologicalSex,
    decimal? HeightCm,
    string? ActivityLevel,
    string? PreferredUnits,
    string? PreferredLanguage,
    string[]? MedicalConditions);
