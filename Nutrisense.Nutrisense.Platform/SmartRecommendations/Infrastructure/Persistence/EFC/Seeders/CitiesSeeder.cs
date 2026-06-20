using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Seeders;

public class CitiesSeeder(AppDbContext context)
{
    public async Task SeedAsync()
    {
        if (await context.Cities.AnyAsync()) return;

        var cities = new List<City>
        {
            new("lima", "Lima", "Lima", "PE", -12.046m, -77.043m, "America/Lima"),
            new("cusco", "Cusco", "Cusco", "PE", -13.532m, -71.967m, "America/Lima"),
            new("new-york", "New York", "Nueva York", "US", 40.713m, -74.006m, "America/New_York")
        };

        await context.Cities.AddRangeAsync(cities);
        await context.SaveChangesAsync();
    }
}
