using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;

public interface IFoodImportCommandService
{
    /// <summary>Returns the number of new foods imported and persisted.</summary>
    Task<Result<int, NutritionTrackingError>> Handle(ImportFoodsCommand command, CancellationToken ct = default);
}
