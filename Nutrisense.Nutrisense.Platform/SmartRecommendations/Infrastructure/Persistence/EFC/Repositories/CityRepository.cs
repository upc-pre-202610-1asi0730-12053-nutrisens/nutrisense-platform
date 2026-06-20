using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Repositories;

public class CityRepository(AppDbContext context)
    : BaseRepository<City>(context), ICityRepository
{
    public async Task<City?> FindByKeyAsync(string key, CancellationToken ct = default) =>
        await Context.Set<City>().FirstOrDefaultAsync(c => c.Key == key, ct);

    public async Task<City?> FindNearestAsync(decimal lat, decimal lng, CancellationToken ct = default)
    {
        var cities = await Context.Set<City>().ToListAsync(ct);
        return cities
            .Select(c => new
            {
                City = c,
                Distance = Math.Sqrt(
                    Math.Pow((double)(lat - c.Lat), 2) +
                    Math.Pow((double)(lng - c.Lng), 2))
            })
            .OrderBy(x => x.Distance)
            .FirstOrDefault()?.City;
    }

    Task<City?> IBaseRepository<City>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
