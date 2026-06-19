using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class SetDietaryRestrictionsCommandAssembler
{
    public static SetDietaryRestrictionsCommand ToCommand(UserId userId, SetDietaryRestrictionsResource resource) =>
        new(userId, resource.Restrictions);
}
