using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class RegisterUserCommandAssembler
{
    public static RegisterUserCommand ToCommand(RegisterUserResource resource) =>
        new(resource.Email, resource.Password, resource.FirstName, resource.LastName, resource.PreferredLanguage);
}
