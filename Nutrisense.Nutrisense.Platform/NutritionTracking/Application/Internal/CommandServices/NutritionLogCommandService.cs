using Cortex.Mediator;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal.CommandServices;

public class NutritionLogCommandService(
    INutritionLogRepository nutritionLogRepository,
    IFoodRepository foodRepository,
    IUnitOfWork unitOfWork,
    IDishVisionService dishVisionService,
    IMenuVisionService menuVisionService,
    IFoodNutritionEstimationService foodEstimationService,
    IFoodCommandService foodCommandService,
    ILogger<NutritionLogCommandService> logger,
    IMediator mediator) : INutritionLogCommandService
{
    private const string DishScanSource = "ai-dish-scan";
    private const string MenuScanSource = "ai-menu-scan";

    public async Task<Result<NutritionLog, NutritionTrackingError>> Handle(UpdateNutritionLogEntryCommand command, CancellationToken ct = default)
    {
        try
        {
            var log = await nutritionLogRepository.FindByIdAsync(command.EntryId, ct);
            if (log is null)
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.EntryNotFound);

            if (log.UserId != command.UserId)
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.EntryUpdateForbidden);

            var food = await foodRepository.FindByIdAsync(log.FoodId, ct);
            if (food is null)
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.EntryNotFound);

            try
            {
                log.UpdateQuantity(food, command.QuantityG);
            }
            catch (ArgumentException)
            {
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.InvalidEntryQuantity);
            }

            nutritionLogRepository.Update(log);
            await unitOfWork.CompleteAsync(ct);
            await PublishConsumptionUpdated(log.UserId, log.Date, ct);

            return new Result<NutritionLog, NutritionTrackingError>.Success(log);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating nutrition log entry {EntryId}", command.EntryId);
            return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }

    public async Task<Result<bool, NutritionTrackingError>> Handle(DeleteNutritionLogEntryCommand command, CancellationToken ct = default)
    {
        try
        {
            var log = await nutritionLogRepository.FindByIdAsync(command.EntryId, ct);
            if (log is null)
                return new Result<bool, NutritionTrackingError>.Failure(NutritionTrackingError.EntryNotFound);

            if (log.UserId != command.UserId)
                return new Result<bool, NutritionTrackingError>.Failure(NutritionTrackingError.EntryDeleteForbidden);

            var date = log.Date;
            nutritionLogRepository.Remove(log);
            await unitOfWork.CompleteAsync(ct);
            await PublishConsumptionUpdated(command.UserId, date, ct);

            return new Result<bool, NutritionTrackingError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting nutrition log entry {EntryId}", command.EntryId);
            return new Result<bool, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }

    public async Task<Result<NutritionLog, NutritionTrackingError>> Handle(LogMealToDailyLogCommand command, CancellationToken ct = default)
    {
        try
        {
            var food = await foodRepository.FindByIdAsync(command.FoodId, ct);
            if (food is null)
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.FoodNotFound);

            NutritionLog log;
            try
            {
                log = new NutritionLog(command, food);
            }
            catch (ArgumentException)
            {
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.InvalidMealType);
            }

            await nutritionLogRepository.AddAsync(log, ct);
            await unitOfWork.CompleteAsync(ct);
            await PublishConsumptionUpdated(command.UserId, command.Date, ct);

            return new Result<NutritionLog, NutritionTrackingError>.Success(log);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging meal for user {UserId}", command.UserId);
            return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }

    public async Task<Result<ScanPreviewResult, NutritionTrackingError>> Handle(ScanMealPhotoCommand command, CancellationToken ct = default)
    {
        try
        {
            // 1) Gemini vision: recognize the items present on the plate.
            var recognition = await dishVisionService.RecognizeDishAsync(command.ImageBase64OrUri, ct);
            if (!recognition.Success)
                return new Result<ScanPreviewResult, NutritionTrackingError>.Failure(NutritionTrackingError.DishScanFailed);

            // Nothing detected: succeed with an empty preview so the client shows its dish fallback.
            if (recognition.Items.Count == 0)
                return new Result<ScanPreviewResult, NutritionTrackingError>.Success(new ScanPreviewResult([]));

            var items = new List<ScannedDishItem>();
            var unmatched = new List<DetectedDishItem>();

            // 2) Resolve each detected item against the local catalog.
            foreach (var detected in recognition.Items)
            {
                var match = (await foodRepository.SearchByNameAsync(detected.Name, "en", ct)).FirstOrDefault();
                if (match is not null)
                    items.Add(new ScannedDishItem(
                        match.Id, match.NameEn, match.NameEs, detected.EstimatedQuantityG,
                        match.CaloriesPer100g, match.ProteinPer100g, match.CarbsPer100g, match.FatPer100g,
                        IsEstimate: false));
                else
                    unmatched.Add(detected);
            }

            // 3) Estimate macros for the unmatched items (DeepSeek), persist them as new foods, and add to the preview.
            if (unmatched.Count > 0)
            {
                var estimates = await foodEstimationService.EstimateBatchAsync(
                    unmatched.Select(u => u.Name).ToList(), ct);

                for (var i = 0; i < unmatched.Count; i++)
                {
                    var est = i < estimates.Count ? estimates[i] : null;
                    var detected = unmatched[i];
                    if (est is null) continue;

                    var foodId = await ResolveOrCreateFoodAsync(est, DishScanSource, ct);
                    items.Add(new ScannedDishItem(
                        foodId, est.NameEn, est.NameEs, detected.EstimatedQuantityG,
                        est.CaloriesPer100g, est.ProteinPer100g, est.CarbsPer100g, est.FatPer100g,
                        IsEstimate: true));
                }
            }

            var firstFoodId = items.FirstOrDefault(i => i.FoodId is not null)?.FoodId ?? 0;
            await mediator.PublishAsync(new MealPhotoAnalyzed(command.UserId, firstFoodId, Confidence: 1m));

            return new Result<ScanPreviewResult, NutritionTrackingError>.Success(new ScanPreviewResult(items));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error scanning meal photo for user {UserId}", command.UserId);
            return new Result<ScanPreviewResult, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }

    /// <summary>
    /// Materializes an AI-estimated food into the catalog (cache) and returns its id. If the food
    /// already exists (deduped by key), it is looked up by name instead. Returns null only if it can
    /// neither be created nor found.
    /// </summary>
    private async Task<int?> ResolveOrCreateFoodAsync(EstimatedFoodData est, string source, CancellationToken ct)
    {
        var register = new RegisterFoodCommand(
            est.NameEn, est.NameEs, est.Category, source, ExternalId: null,
            est.ServingSizeG, est.ServingUnit,
            est.CaloriesPer100g, est.ProteinPer100g, est.CarbsPer100g,
            est.FatPer100g, est.FiberPer100g, est.SugarPer100g,
            est.Restrictions.ToArray());

        var result = await foodCommandService.Handle(register, ct);
        if (result is Result<Food, NutritionTrackingError>.Success created)
            return created.Value.Id;

        // Already cached (DuplicateKey) or could not be registered: fall back to a name lookup.
        var existing = (await foodRepository.SearchByNameAsync(est.NameEn, "en", ct)).FirstOrDefault();
        return existing?.Id;
    }

    public async Task<Result<NutritionLog, NutritionTrackingError>> Handle(ConfirmScanResultCommand command, CancellationToken ct = default)
    {
        try
        {
            var food = await foodRepository.FindByIdAsync(command.DetectedFoodId, ct);
            if (food is null)
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.FoodNotFound);

            NutritionLog log;
            try
            {
                log = new NutritionLog(command, food, confidence: 0.87m, imageUri: string.Empty);
            }
            catch (ArgumentException)
            {
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.InvalidScanConfirmationData);
            }

            await nutritionLogRepository.AddAsync(log, ct);
            await unitOfWork.CompleteAsync(ct);
            await PublishConsumptionUpdated(command.UserId, command.Date, ct);

            return new Result<NutritionLog, NutritionTrackingError>.Success(log);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error confirming scan result for user {UserId}", command.UserId);
            return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }

    public async Task<Result<MenuOptionsPreview, NutritionTrackingError>> Handle(ScanMenuPhotoCommand command, CancellationToken ct = default)
    {
        try
        {
            // 1) Gemini vision/OCR: extract the dishes listed on the menu.
            var recognition = await menuVisionService.AnalyzeMenuAsync(command.ImageBase64OrUri, ct);
            if (!recognition.Success)
                return new Result<MenuOptionsPreview, NutritionTrackingError>.Failure(NutritionTrackingError.MenuScanFailed);

            var dishes = recognition.Options.ToList();

            // Nothing detected: succeed with an empty preview so the client shows its menu fallback.
            if (dishes.Count == 0)
                return new Result<MenuOptionsPreview, NutritionTrackingError>.Success(new MenuOptionsPreview([]));

            var options = new List<ScannedMenuOption>();
            var unmatched = new List<string>();

            // 2) Resolve each dish against the local catalog.
            foreach (var dish in dishes)
            {
                var match = (await foodRepository.SearchByNameAsync(dish.DishName, "en", ct)).FirstOrDefault();
                if (match is not null)
                    options.Add(new ScannedMenuOption(
                        match.Id, match.NameEn, match.NameEs,
                        match.CaloriesPer100g, match.ProteinPer100g, match.CarbsPer100g, match.FatPer100g,
                        match.Restrictions, IsEstimate: false));
                else
                    unmatched.Add(dish.DishName);
            }

            // 3) Estimate + cache the unmatched dishes as new foods (DeepSeek), then add to the preview.
            if (unmatched.Count > 0)
            {
                var estimates = await foodEstimationService.EstimateBatchAsync(unmatched, ct);
                foreach (var est in estimates)
                {
                    var foodId = await ResolveOrCreateFoodAsync(est, MenuScanSource, ct);
                    options.Add(new ScannedMenuOption(
                        foodId, est.NameEn, est.NameEs,
                        est.CaloriesPer100g, est.ProteinPer100g, est.CarbsPer100g, est.FatPer100g,
                        est.Restrictions, IsEstimate: true));
                }
            }

            var detectedFoodIds = options.Where(o => o.FoodId is not null).Select(o => o.FoodId!.Value).ToArray();
            await mediator.PublishAsync(new MenuAnalyzed(command.UserId, detectedFoodIds));

            return new Result<MenuOptionsPreview, NutritionTrackingError>.Success(new MenuOptionsPreview(options));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error scanning menu photo for user {UserId}", command.UserId);
            return new Result<MenuOptionsPreview, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }

    public async Task<Result<NutritionLog, NutritionTrackingError>> Handle(SelectMenuOptionCommand command, CancellationToken ct = default)
    {
        try
        {
            var food = await foodRepository.FindByIdAsync(command.FoodId, ct);
            if (food is null)
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.FoodNotFound);

            NutritionLog log;
            try
            {
                log = new NutritionLog(command, food);
            }
            catch (ArgumentException)
            {
                return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.InvalidMenuSelectionData);
            }

            await nutritionLogRepository.AddAsync(log, ct);
            await unitOfWork.CompleteAsync(ct);
            await PublishConsumptionUpdated(command.UserId, command.Date, ct);

            return new Result<NutritionLog, NutritionTrackingError>.Success(log);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error selecting menu option for user {UserId}", command.UserId);
            return new Result<NutritionLog, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }

    private async Task PublishConsumptionUpdated(int userId, DateOnly date, CancellationToken ct)
    {
        var summary = await nutritionLogRepository.GetDailyMacroSummaryAsync(userId, date, ct);
        await mediator.PublishAsync(new ConsumptionUpdated(
            userId,
            date,
            summary.TotalCalories,
            summary.TotalProteinG,
            summary.TotalCarbsG,
            summary.TotalFatG,
            summary.TotalFiberG));
    }
}
