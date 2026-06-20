using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Errors;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.CommandServices;

/// <summary>Application service contract for all body-metrics write operations.</summary>
public interface IBodyMetricsCommandService
{
    Task<Result<BodyMetrics, RegisterBodyMetricsError>> Handle(RegisterBodyMetricsCommand command);
    Task<Result<BodyMetrics, UpdateWeightError>> Handle(UpdateWeightCommand command);
    Task<Result<BodyMetrics, RegisterBodyMeasurementError>> Handle(RegisterBodyMeasurementCommand command);
    Task<Result<BodyMetrics, SetHealthGoalError>> Handle(SetHealthGoalCommand command);
    Task<Result<BodyMetrics, CalculateDailyCaloricGoalError>> Handle(CalculateDailyCaloricGoalCommand command);
}
