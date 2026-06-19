using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Events;

public record UserRegistered(UserId UserId, string Email, string PreferredLanguage) : DomainEventBase;
