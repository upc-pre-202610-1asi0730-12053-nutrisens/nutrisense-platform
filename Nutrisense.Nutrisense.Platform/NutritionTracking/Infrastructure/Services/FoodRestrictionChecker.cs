using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Services;

public class FoodRestrictionChecker : IFoodRestrictionChecker
{
    public bool HasConflict(Food food, IEnumerable<string> userRestrictions) =>
        food.Restrictions.Any(r => userRestrictions.Contains(r, StringComparer.OrdinalIgnoreCase));
}
