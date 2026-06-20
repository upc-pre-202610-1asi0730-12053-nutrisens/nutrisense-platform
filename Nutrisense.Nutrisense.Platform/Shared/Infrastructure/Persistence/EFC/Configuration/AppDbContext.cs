using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; init; }

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
