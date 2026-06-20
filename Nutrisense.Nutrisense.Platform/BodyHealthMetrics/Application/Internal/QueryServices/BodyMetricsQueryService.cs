using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Internal.QueryServices;

/// <summary>Handles body-metrics read queries by delegating to the repository.</summary>
public class BodyMetricsQueryService(IBodyMetricsRepository bodyMetricsRepository) : IBodyMetricsQueryService
{
    public async Task<BodyMetrics?> Handle(GetBodyMetricsByUserIdQuery query) =>
        await bodyMetricsRepository.FindByUserIdAsync(query.UserId);

    public async Task<IEnumerable<WeightLog>> Handle(GetWeightHistoryByUserIdQuery query) =>
        await bodyMetricsRepository.FindWeightHistoryByUserIdAsync(query.UserId, query.From, query.To);
}
