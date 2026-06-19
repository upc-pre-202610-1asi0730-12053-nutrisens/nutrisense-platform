namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;

public record LoginUserCommand(
    string Email,
    string Password,
    string? DeviceLabel = null);
