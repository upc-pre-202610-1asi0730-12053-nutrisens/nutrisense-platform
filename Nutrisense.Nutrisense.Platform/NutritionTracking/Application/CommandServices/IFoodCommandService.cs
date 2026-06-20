using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;

public interface IFoodCommandService
{
    Task<Result<Food, RegisterFoodError>> Handle(RegisterFoodCommand command, CancellationToken ct = default);
}
