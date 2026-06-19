using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;

public record LogoutUserCommand(UserId UserId, int SessionId);
