using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class UserResourceAssembler
{
    public static UserResource ToResource(User user) =>
        new(user.Id,
            user.Email.Value,
            user.Name.FirstName,
            user.Name.LastName,
            user.DateOfBirth?.Value.ToString("yyyy-MM-dd"),
            user.BiologicalSex?.Value,
            user.Height?.Centimeters,
            user.GoalIntent?.Value,
            user.ActivityLevel?.Value,
            user.PreferredUnits.Value,
            user.PreferredLanguage.Value,
            user.MedicalConditions.ToArray(),
            user.DietaryRestrictions.Select(dr => dr.Restriction).ToArray());
}
