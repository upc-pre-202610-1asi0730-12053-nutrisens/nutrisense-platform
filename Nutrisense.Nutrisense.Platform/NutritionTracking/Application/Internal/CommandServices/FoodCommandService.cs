using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal.CommandServices;

public class FoodCommandService(
    IFoodRepository foodRepository,
    IUnitOfWork unitOfWork,
    ILogger<FoodCommandService> logger) : IFoodCommandService
{
    public async Task<Result<Food, NutritionTrackingError>> Handle(RegisterFoodCommand command, CancellationToken ct = default)
    {
        try
        {
            Food food;
            try
            {
                food = new Food(command);
            }
            catch (ArgumentException)
            {
                return new Result<Food, NutritionTrackingError>.Failure(NutritionTrackingError.InvalidFoodSource);
            }

            var existing = await foodRepository.FindByKeyAsync(food.Key, ct);
            if (existing is not null)
                return new Result<Food, NutritionTrackingError>.Failure(NutritionTrackingError.FoodDuplicateKey);

            await foodRepository.AddAsync(food, ct);
            await unitOfWork.CompleteAsync(ct);

            return new Result<Food, NutritionTrackingError>.Success(food);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering food '{NameEn}'", command.NameEn);
            return new Result<Food, NutritionTrackingError>.Failure(NutritionTrackingError.UnexpectedError);
        }
    }
}
