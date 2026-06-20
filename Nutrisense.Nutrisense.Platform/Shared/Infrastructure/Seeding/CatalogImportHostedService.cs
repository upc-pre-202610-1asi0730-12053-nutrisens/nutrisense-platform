using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Seeding;

/// <summary>
/// Populates the food/ingredient/recipe catalogs in the background after startup, so the app
/// becomes responsive immediately. Each stage is gated by a configurable count threshold, making
/// the whole thing idempotent across redeploys. Runs only when <c>Seeder:Enabled=true</c>.
/// </summary>
/// <remarks>
/// Guards: (1) the whole run is wrapped in try/catch so an external (USDA/DeepSeek) failure never
/// crashes the host (BackgroundServiceExceptionBehavior.StopHost is the .NET default);
/// (2) scoped services are resolved from a dedicated scope via <see cref="IServiceScopeFactory"/>;
/// (3) the stopping token is propagated to every call; (4) EF migrations run before app start
/// (in Program.cs), so the schema is ready by the time this executes.
/// </remarks>
public class CatalogImportHostedService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<CatalogImportHostedService> logger) : BackgroundService
{
    private static readonly string[] DefaultFoodQueries =
    [
        "chicken breast", "brown rice", "broccoli", "salmon", "oats",
        "eggs", "banana", "black beans", "greek yogurt", "almonds"
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!configuration.GetValue("Seeder:Enabled", false))
        {
            logger.LogInformation("[CatalogImport] Disabled (Seeder:Enabled=false); skipping import.");
            return;
        }

        try
        {
            using var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            await SeedFoodsAsync(sp, stoppingToken);
            await SeedIngredientsAsync(sp, stoppingToken);
            await SeedRecipesAsync(sp, stoppingToken);

            logger.LogInformation("[CatalogImport] Catalog import finished.");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("[CatalogImport] Cancelled during shutdown.");
        }
        catch (Exception ex)
        {
            // Guard (1): never let an import failure take down the host.
            logger.LogError(ex, "[CatalogImport] Import failed; application continues running.");
        }
    }

    private async Task SeedFoodsAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<IFoodRepository>();
        var current = (await repo.ListAsync(ct)).Count();
        var minCount = configuration.GetValue("Seeder:Foods:MinCount", 100);
        if (current >= minCount)
        {
            logger.LogInformation("[CatalogImport] Foods: {Current} present (>= {Min}); skipping.", current, minCount);
            return;
        }

        var queries = configuration.GetSection("Seeder:Foods:Queries").Get<string[]>() ?? DefaultFoodQueries;
        var maxResults = configuration.GetValue("Seeder:Foods:MaxResults", 50);
        var dataType = configuration.GetValue("Seeder:Foods:DataType", "Foundation") ?? "Foundation";

        var importService = sp.GetRequiredService<IFoodImportCommandService>();
        var total = 0;
        foreach (var query in queries)
        {
            ct.ThrowIfCancellationRequested();
            var result = await importService.Handle(new ImportFoodsCommand(query.Trim(), maxResults, dataType), ct);
            result.Match(
                count => total += count,
                error => logger.LogWarning("[CatalogImport] Foods '{Query}' failed: {Error}", query, error));
        }

        logger.LogInformation("[CatalogImport] Foods: import complete — {Total} new items.", total);
    }

    private async Task SeedIngredientsAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<IIngredientCatalogRepository>();
        var current = (await repo.ListAsync(ct)).Count();
        var minCount = configuration.GetValue("Seeder:Ingredients:MinCount", 1);
        if (current >= minCount)
        {
            logger.LogInformation("[CatalogImport] Ingredients: {Current} present (>= {Min}); skipping.", current, minCount);
            return;
        }

        var service = sp.GetRequiredService<IIngredientCatalogImportCommandService>();
        var derived = await service.Handle(new DeriveIngredientsFromFoodsCommand(), ct);
        logger.LogInformation("[CatalogImport] Ingredients: derived {Derived} from foods.", derived);
    }

    private async Task SeedRecipesAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<IRecipeRepository>();
        var current = (await repo.ListAsync(ct)).Count();
        var minCount = configuration.GetValue("Seeder:Recipes:MinCount", 20);
        if (current >= minCount)
        {
            logger.LogInformation("[CatalogImport] Recipes: {Current} present (>= {Min}); skipping.", current, minCount);
            return;
        }

        var maxPerGoal = configuration.GetValue("Seeder:Recipes:MaxPerGoal", 10);
        var service = sp.GetRequiredService<IRecipeImportCommandService>();
        var result = await service.Handle(new ImportRecipeSuggestionsCommand(null, maxPerGoal), ct);
        result.Match(
            generated => logger.LogInformation("[CatalogImport] Recipes: generated {Generated}.", generated),
            error => logger.LogWarning("[CatalogImport] Recipes: generation failed — {Error}.", error));
    }
}
