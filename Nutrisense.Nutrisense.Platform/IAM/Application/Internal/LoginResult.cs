using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Application.Internal;

public record LoginResult(UserId UserId, string Token, int SessionId);
