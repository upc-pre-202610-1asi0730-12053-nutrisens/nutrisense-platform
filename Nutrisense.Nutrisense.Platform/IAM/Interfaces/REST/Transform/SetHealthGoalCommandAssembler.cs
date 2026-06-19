using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class SetHealthGoalCommandAssembler
{
    public static SetHealthGoalCommand ToCommand(UserId userId, SetHealthGoalResource resource) =>
        new(userId, resource.Goal);
}
