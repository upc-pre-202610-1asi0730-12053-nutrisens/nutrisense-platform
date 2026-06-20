using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;

public interface INutritionLogCommandService
{
    Task<Result<NutritionLog, LogMealError>> Handle(LogMealToDailyLogCommand command, CancellationToken ct = default);
    Task<Result<NutritionLog, UpdateNutritionLogEntryError>> Handle(UpdateNutritionLogEntryCommand command, CancellationToken ct = default);
    Task<Result<bool, DeleteNutritionLogEntryError>> Handle(DeleteNutritionLogEntryCommand command, CancellationToken ct = default);
    Task<Result<ScanPreviewResult, ScanMealPhotoError>> Handle(ScanMealPhotoCommand command, CancellationToken ct = default);
    Task<Result<NutritionLog, ConfirmScanError>> Handle(ConfirmScanResultCommand command, CancellationToken ct = default);
    Task<Result<MenuOptionsPreview, ScanMenuPhotoError>> Handle(ScanMenuPhotoCommand command, CancellationToken ct = default);
    Task<Result<NutritionLog, SelectMenuOptionError>> Handle(SelectMenuOptionCommand command, CancellationToken ct = default);
}
