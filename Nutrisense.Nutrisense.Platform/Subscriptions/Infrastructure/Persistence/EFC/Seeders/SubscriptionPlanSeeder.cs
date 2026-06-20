using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Seeders;

public class SubscriptionPlanSeeder(AppDbContext context)
{
    public async Task SeedAsync()
    {
        var seed = new[]
        {
            new { Key = "basic",   PriceMonthly = 7.99m,  PriceAnnual = (decimal?)76.70m,
                  Features = new List<string> { "nutrition-log-manual", "dashboard", "bmi-bmr-tdee", "history-30d" } },
            new { Key = "pro",     PriceMonthly = 14.99m, PriceAnnual = (decimal?)143.90m,
                  Features = new List<string> { "nutrition-log-manual", "dashboard", "bmi-bmr-tdee", "history-30d",
                      "history-90d", "smart-scan-dish", "travel-mode",
                      "weather-recommendations", "pantry-recipes" } },
            new { Key = "premium", PriceMonthly = 19.99m, PriceAnnual = (decimal?)191.90m,
                  Features = new List<string> { "nutrition-log-manual", "dashboard", "bmi-bmr-tdee", "history-30d",
                      "history-90d", "smart-scan-dish", "travel-mode",
                      "weather-recommendations", "pantry-recipes",
                      "wearable-sync", "smart-scan-menu", "history-unlimited", "pdf-reports" } },
        };

        var existingKeys = await context.SubscriptionPlans
            .Select(p => p.Key)
            .ToHashSetAsync();

        foreach (var s in seed)
        {
            var featuresJson = System.Text.Json.JsonSerializer.Serialize(s.Features);

            if (!existingKeys.Contains(s.Key))
            {
                context.SubscriptionPlans.Add(
                    new SubscriptionPlan(s.Key, s.PriceMonthly, s.Features, priceAnnual: s.PriceAnnual));
                await context.SaveChangesAsync();
            }
            else
            {
                // Overwrite features and prices in-place so stale rows are corrected on every startup.
                await context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE subscription_plans SET features = {featuresJson}, price_monthly = {s.PriceMonthly}, price_annual = {s.PriceAnnual} WHERE `key` = {s.Key}");
            }
        }
    }
}
