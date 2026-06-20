using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Acl;

/// <inheritdoc cref="INutritionTrackingContextFacade"/>
public class NutritionTrackingContextFacade(
    IFoodQueryService foodQueryService,
    IFoodProvisioningService foodProvisioningService,
    INutritionLogQueryService nutritionLogQueryService) : INutritionTrackingContextFacade
{
    public async Task<IReadOnlyList<FoodCatalogItem>> GetFoodCatalog(CancellationToken ct = default)
    {
        // An empty search term matches every food, so this yields the full catalog.
        var foods = await foodQueryService.Handle(new SearchFoodQuery(string.Empty, "en"), ct);
        return foods
            .Select(f => new FoodCatalogItem(
                f.Id, f.Key, f.NameEn, f.NameEs, f.Category, f.ServingUnit, f.ServingSizeG))
            .ToList();
    }

    public async Task<IReadOnlyList<ProvisionedFoodItem>> ResolveOrCreateFoodsByNames(
        IReadOnlyList<string> namesEn, string source, CancellationToken ct = default)
    {
        var provisioned = await foodProvisioningService.ResolveOrCreateByNamesAsync(namesEn, source, ct);
        return provisioned
            .Select(p => new ProvisionedFoodItem(
                p.Id, p.NameEn, p.NameEs, p.ServingSizeG, p.ServingUnit,
                p.CaloriesPer100g, p.ProteinPer100g, p.CarbsPer100g, p.FatPer100g, p.Restrictions))
            .ToList();
    }

    public async Task<DailyMacroSummaryItem?> GetDailyMacroSummary(
        int userId, DateOnly date, CancellationToken ct = default)
    {
        var summary = await nutritionLogQueryService.Handle(new GetDailyMacroSummaryQuery(userId, date), ct);
        if (summary is null || summary.MealCount == 0) return null;

        return new DailyMacroSummaryItem(
            summary.TotalCalories,
            summary.TotalProteinG,
            summary.TotalCarbsG,
            summary.TotalFatG,
            summary.TotalFiberG);
    }

    public async Task<IReadOnlyList<string>> GetLoggedMealTypes(
        int userId, DateOnly date, CancellationToken ct = default)
    {
        var logs = await nutritionLogQueryService.Handle(new GetNutritionLogByUserAndDateQuery(userId, date), ct);
        return logs.Select(l => l.MealType).Distinct().ToList();
    }
}
