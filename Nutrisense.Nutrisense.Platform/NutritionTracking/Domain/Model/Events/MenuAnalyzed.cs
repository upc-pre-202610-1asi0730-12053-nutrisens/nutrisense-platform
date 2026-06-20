using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Events;

public record MenuAnalyzed(int UserId, int[] DetectedFoodIds) : DomainEventBase;
