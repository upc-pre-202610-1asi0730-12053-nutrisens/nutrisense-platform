using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Errors;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;

public interface IRecsEngineCommandService
{
    Task<Result<RecommendationCard, SmartRecommendationsError>> Handle(GenerateRecommendationCommand command, CancellationToken ct = default);
    Task<Result<LocationPreference, SmartRecommendationsError>> Handle(EnableTravelModeCommand command, CancellationToken ct = default);
    Task<Result<LocationPreference, SmartRecommendationsError>> Handle(DisableTravelModeCommand command, CancellationToken ct = default);
    Task<Result<Pantry, SmartRecommendationsError>> Handle(RegisterPantryItemsCommand command, CancellationToken ct = default);
    Task<Result<Pantry, SmartRecommendationsError>> Handle(RemovePantryItemCommand command, CancellationToken ct = default);
    Task<Result<Pantry, SmartRecommendationsError>> Handle(UpdatePantryItemCommand command, CancellationToken ct = default);
    Task<Result<Recipe, SmartRecommendationsError>> Handle(SuggestRecipeCommand command, CancellationToken ct = default);
    Task<Result<LocationPreference, SmartRecommendationsError>> Handle(DetectLocationCommand command, CancellationToken ct = default);
    Task Handle(UnlockPremiumFeaturesCommand command, CancellationToken ct = default);
    Task Handle(LockPremiumFeaturesCommand command, CancellationToken ct = default);
    Task<Result<LocationPreference, SmartRecommendationsError>> Handle(SetHomeCityCommand command, CancellationToken ct = default);
    Task<LocationPreference> Handle(SetLocationPermissionCommand command, CancellationToken ct = default);
    Task<Result<City, SmartRecommendationsError>> Handle(ImportCityCommand command, CancellationToken ct = default);
}
