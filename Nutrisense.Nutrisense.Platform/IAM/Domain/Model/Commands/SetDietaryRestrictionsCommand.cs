using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;

public record SetDietaryRestrictionsCommand(UserId UserId, string[] Restrictions);
