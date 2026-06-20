using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal.CommandServices;

/// <inheritdoc cref="IFoodProvisioningService"/>
public class FoodProvisioningService(
    IFoodRepository foodRepository,
    IFoodNutritionEstimationService foodEstimationService,
    IFoodCommandService foodCommandService,
    ILogger<FoodProvisioningService> logger) : IFoodProvisioningService
{
    public async Task<IReadOnlyList<ProvisionedFood>> ResolveOrCreateByNamesAsync(
        IReadOnlyList<string> namesEn, string source, CancellationToken ct = default)
    {
        if (namesEn.Count == 0) return [];

        var resolved = new List<ProvisionedFood>(namesEn.Count);
        var unmatched = new List<string>();
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 1) Catalog-first: anything already known is used as-is, no LLM call.
        foreach (var name in namesEn)
        {
            if (string.IsNullOrWhiteSpace(name) || !seenNames.Add(name.Trim())) continue;

            var match = (await foodRepository.SearchByNameAsync(name.Trim(), "en", ct)).FirstOrDefault();
            if (match is not null)
                resolved.Add(ToProvisioned(match));
            else
                unmatched.Add(name.Trim());
        }

        // 2) Estimate the misses in one batch and persist each as a new food (deduped by key).
        if (unmatched.Count > 0)
        {
            var estimates = await foodEstimationService.EstimateBatchAsync(unmatched, ct);
            foreach (var est in estimates)
            {
                var food = await ResolveOrCreateAsync(est, source, ct);
                if (food is not null)
                    resolved.Add(food);
            }
        }

        return resolved;
    }

    /// <summary>
    /// Materializes an AI-estimated food into the catalog and returns it. If it already exists (deduped
    /// by key) it is looked up by name instead. Returns null only if it can neither be created nor found.
    /// </summary>
    private async Task<ProvisionedFood?> ResolveOrCreateAsync(EstimatedFoodData est, string source, CancellationToken ct)
    {
        var register = new RegisterFoodCommand(
            est.NameEn, est.NameEs, est.Category, source, ExternalId: null,
            est.ServingSizeG, est.ServingUnit,
            est.CaloriesPer100g, est.ProteinPer100g, est.CarbsPer100g,
            est.FatPer100g, est.FiberPer100g, est.SugarPer100g,
            est.Restrictions.ToArray());

        var result = await foodCommandService.Handle(register, ct);
        if (result is Result<Food, RegisterFoodError>.Success created)
            return ToProvisioned(created.Value);

        // Already cached (DuplicateKey) or could not be registered: fall back to a name lookup.
        var existing = (await foodRepository.SearchByNameAsync(est.NameEn, "en", ct)).FirstOrDefault();
        if (existing is not null)
            return ToProvisioned(existing);

        logger.LogWarning("[FoodProvisioning] Could not resolve or create food '{NameEn}'.", est.NameEn);
        return null;
    }

    private static ProvisionedFood ToProvisioned(Food f) =>
        new(f.Id, f.NameEn, f.NameEs, f.ServingSizeG, f.ServingUnit,
            f.CaloriesPer100g, f.ProteinPer100g, f.CarbsPer100g, f.FatPer100g, f.Restrictions);
}
