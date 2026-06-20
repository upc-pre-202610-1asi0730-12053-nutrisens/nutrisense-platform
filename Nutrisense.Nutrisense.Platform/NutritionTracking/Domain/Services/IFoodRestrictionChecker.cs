using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

public interface IFoodRestrictionChecker
{
    bool HasConflict(Food food, IEnumerable<string> userRestrictions);
}
