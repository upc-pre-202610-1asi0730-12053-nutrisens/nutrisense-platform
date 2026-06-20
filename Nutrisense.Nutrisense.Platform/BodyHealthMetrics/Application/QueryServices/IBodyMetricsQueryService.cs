using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.QueryServices;

/// <summary>Application service contract for body-metrics read operations.</summary>
public interface IBodyMetricsQueryService
{
    Task<BodyMetrics?> Handle(GetBodyMetricsByUserIdQuery query);
    Task<IEnumerable<WeightLog>> Handle(GetWeightHistoryByUserIdQuery query);
}
