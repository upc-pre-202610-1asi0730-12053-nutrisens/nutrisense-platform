namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;

public record ResetPasswordCommand(string Token, string NewPassword);
