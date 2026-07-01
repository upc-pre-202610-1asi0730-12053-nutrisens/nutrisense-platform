using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.CommandServices;

/// <summary>Application service contract for all body-metrics write operations.</summary>
public interface IBodyMetricsCommandService
{
    Task<Result<BodyMetrics, BodyHealthMetricsError>> Handle(RegisterBodyMetricsCommand command);
    Task<Result<BodyMetrics, BodyHealthMetricsError>> Handle(UpdateWeightCommand command);
    Task<Result<BodyMetrics, BodyHealthMetricsError>> Handle(RegisterBodyMeasurementCommand command);
    Task<Result<BodyMetrics, BodyHealthMetricsError>> Handle(SetHealthGoalCommand command);
    Task<Result<BodyMetrics, BodyHealthMetricsError>> Handle(CalculateDailyCaloricGoalCommand command);
}
