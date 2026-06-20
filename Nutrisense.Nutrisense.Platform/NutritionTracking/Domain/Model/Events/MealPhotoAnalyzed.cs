using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Events;

public record MealPhotoAnalyzed(int UserId, int DetectedFoodId, decimal Confidence) : DomainEventBase;
