using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class LoginUserCommandAssembler
{
    public static LoginUserCommand ToCommand(LoginUserResource resource) =>
        new(resource.Email, resource.Password, resource.DeviceLabel);
}
