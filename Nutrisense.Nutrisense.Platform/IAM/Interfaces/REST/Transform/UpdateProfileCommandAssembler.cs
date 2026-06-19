using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class UpdateProfileCommandAssembler
{
    public static UpdateProfileCommand ToCommand(UserId userId, UpdateProfileResource resource)
    {
        DateOnly? dateOfBirth = null;
        if (resource.DateOfBirth is not null &&
            DateOnly.TryParseExact(resource.DateOfBirth, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var parsed))
        {
            dateOfBirth = parsed;
        }

        return new UpdateProfileCommand(
            userId,
            resource.FirstName,
            resource.LastName,
            dateOfBirth,
            resource.BiologicalSex,
            resource.HeightCm,
            resource.ActivityLevel,
            resource.PreferredUnits,
            resource.PreferredLanguage,
            resource.MedicalConditions);
    }
}
