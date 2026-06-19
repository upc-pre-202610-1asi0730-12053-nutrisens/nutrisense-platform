using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;

public record SetHealthGoalCommand(UserId UserId, string Goal);
