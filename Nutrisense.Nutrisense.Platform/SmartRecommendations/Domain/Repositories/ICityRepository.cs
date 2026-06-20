using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

public interface ICityRepository : IBaseRepository<City>
{
    Task<City?> FindByKeyAsync(string key, CancellationToken ct = default);
    Task<City?> FindNearestAsync(decimal lat, decimal lng, CancellationToken ct = default);
}
