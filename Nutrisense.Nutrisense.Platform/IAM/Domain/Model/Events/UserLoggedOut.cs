using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Events;

public record UserLoggedOut(UserId UserId, int SessionId) : DomainEventBase;
