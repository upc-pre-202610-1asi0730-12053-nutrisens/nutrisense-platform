using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

public interface IRecommendationCardRepository : IBaseRepository<RecommendationCard>
{
    Task<RecommendationCard?> FindActiveByUserIdAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<RecommendationCard>> FindActiveListByUserIdAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<RecommendationCard>> FindByUserIdAsync(int userId, CancellationToken ct = default);
    Task DeactivateAllByUserIdAsync(int userId, CancellationToken ct = default);
}
