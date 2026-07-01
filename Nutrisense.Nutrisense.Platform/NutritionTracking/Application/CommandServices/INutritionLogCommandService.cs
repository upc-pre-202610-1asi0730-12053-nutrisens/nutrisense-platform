using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;

public interface INutritionLogCommandService
{
    Task<Result<NutritionLog, NutritionTrackingError>> Handle(LogMealToDailyLogCommand command, CancellationToken ct = default);
    Task<Result<NutritionLog, NutritionTrackingError>> Handle(UpdateNutritionLogEntryCommand command, CancellationToken ct = default);
    Task<Result<bool, NutritionTrackingError>> Handle(DeleteNutritionLogEntryCommand command, CancellationToken ct = default);
    Task<Result<ScanPreviewResult, NutritionTrackingError>> Handle(ScanMealPhotoCommand command, CancellationToken ct = default);
    Task<Result<NutritionLog, NutritionTrackingError>> Handle(ConfirmScanResultCommand command, CancellationToken ct = default);
    Task<Result<MenuOptionsPreview, NutritionTrackingError>> Handle(ScanMenuPhotoCommand command, CancellationToken ct = default);
    Task<Result<NutritionLog, NutritionTrackingError>> Handle(SelectMenuOptionCommand command, CancellationToken ct = default);
}
