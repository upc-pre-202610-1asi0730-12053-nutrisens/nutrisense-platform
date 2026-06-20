using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;

/// <summary>Domain event raised after a user's BMI is computed or updated.</summary>
public record BmiCalculated(int UserId, decimal Bmi) : DomainEventBase;
