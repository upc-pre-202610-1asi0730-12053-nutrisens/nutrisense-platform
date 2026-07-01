using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class PasswordResetCommandAssembler
{
    public static RequestPasswordResetCommand ToCommand(ForgotPasswordResource resource) =>
        new(resource.Email);

    public static ResetPasswordCommand ToCommand(ResetPasswordResource resource) =>
        new(resource.Token, resource.NewPassword);
}
