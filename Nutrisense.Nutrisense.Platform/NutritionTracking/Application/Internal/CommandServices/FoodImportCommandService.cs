using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal.CommandServices;

/// <summary>
/// Imports foods from the external catalog into the local cache:
/// search → skip already-imported (by external id) → enrich names/category/restrictions
/// in batches → persist via <see cref="IFoodCommandService"/> (which dedups by key).
/// </summary>
public class FoodImportCommandService(
    IExternalFoodDataProvider externalFoodDataProvider,
    IFoodEnrichmentService foodEnrichmentService,
    IFoodCommandService foodCommandService,
    IFoodRepository foodRepository,
    ILogger<FoodImportCommandService> logger) : IFoodImportCommandService
{
    private const int BatchSize = 20;
    private const string Source = "usda";

    public async Task<Result<int, NutritionTrackingError>> Handle(ImportFoodsCommand command, CancellationToken ct = default)
    {
        IReadOnlyList<ExternalFoodData> external;
        try
        {
            external = await externalFoodDataProvider.SearchAsync(command.Query, command.MaxResults, command.DataType, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[FoodImport] USDA search failed for query '{Query}'", command.Query);
            return new Result<int, NutritionTrackingError>.Failure(NutritionTrackingError.UsdaUnavailable);
        }

        logger.LogInformation("[FoodImport] USDA returned {Count} foods for query '{Query}'", external.Count, command.Query);

        try
        {
            // Skip foods already imported (deduped by external id) so we don't re-enrich them.
            var newFoods = new List<ExternalFoodData>();
            foreach (var food in external)
            {
                if (!string.IsNullOrWhiteSpace(food.ExternalId) &&
                    await foodRepository.FindByExternalIdAsync(food.ExternalId!, ct) is not null)
                    continue;
                newFoods.Add(food);
            }

            logger.LogInformation("[FoodImport] {New} new foods after dedup ({Existing} already present)",
                newFoods.Count, external.Count - newFoods.Count);

            var imported = 0;
            for (var i = 0; i < newFoods.Count; i += BatchSize)
            {
                var batch = newFoods.GetRange(i, Math.Min(BatchSize, newFoods.Count - i));
                var names = batch.Select(f => f.Name).ToList();

                IReadOnlyList<EnrichedFoodData> enriched;
                try
                {
                    enriched = await foodEnrichmentService.EnrichBatchAsync(names, ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "[FoodImport] Enrichment failed for batch starting at {Index}; using raw names", i);
                    enriched = names.Select(n => new EnrichedFoodData(n, n, "Other", [])).ToList();
                }

                for (var j = 0; j < batch.Count; j++)
                {
                    var raw = batch[j];
                    var enr = j < enriched.Count ? enriched[j] : new EnrichedFoodData(raw.Name, raw.Name, "Other", []);

                    var register = new RegisterFoodCommand(
                        enr.NameEn, enr.NameEs, enr.Category, Source, raw.ExternalId,
                        raw.ServingSize, raw.ServingUnit,
                        raw.CaloriesPer100g, raw.ProteinPer100g, raw.CarbsPer100g,
                        raw.FatPer100g, raw.FiberPer100g, raw.SugarPer100g,
                        enr.Restrictions.ToArray());

                    var result = await foodCommandService.Handle(register, ct);
                    result.Match(
                        _ => imported++,
                        error => logger.LogDebug("[FoodImport] Skipped '{Name}': {Error}", enr.NameEn, error));
                }

                logger.LogInformation("[FoodImport] Batch {Batch}: running total {Total}", (i / BatchSize) + 1, imported);
            }

            logger.LogInformation("[FoodImport] Complete: {Total} foods imported for query '{Query}'", imported, command.Query);
            return new Result<int, NutritionTrackingError>.Success(imported);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[FoodImport] Unexpected error importing query '{Query}'", command.Query);
            return new Result<int, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }
}
