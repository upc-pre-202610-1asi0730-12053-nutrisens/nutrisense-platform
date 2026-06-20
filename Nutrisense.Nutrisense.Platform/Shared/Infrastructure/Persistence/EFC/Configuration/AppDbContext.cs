using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; init; }
    public DbSet<Food> Foods { get; init; }
    public DbSet<Recipe> Recipes { get; init; }
    public DbSet<RecipeIngredientItem> RecipeIngredientItems { get; init; }
    public DbSet<IngredientCatalogItem> IngredientCatalogItems { get; init; }
    public DbSet<City> Cities { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.UseSnakeCaseNamingConvention();
    }
}
