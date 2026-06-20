using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Errors;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;

public interface IRecsEngineCommandService
{
    Task<Result<RecommendationCard, GenerateRecommendationError>> Handle(GenerateRecommendationCommand command, CancellationToken ct = default);
    Task<Result<LocationPreference, EnableTravelModeError>> Handle(EnableTravelModeCommand command, CancellationToken ct = default);
    Task<Result<LocationPreference, DisableTravelModeError>> Handle(DisableTravelModeCommand command, CancellationToken ct = default);
    Task<Result<Pantry, RegisterPantryItemsError>> Handle(RegisterPantryItemsCommand command, CancellationToken ct = default);
    Task<Result<Pantry, RemovePantryItemError>> Handle(RemovePantryItemCommand command, CancellationToken ct = default);
    Task<Result<Pantry, UpdatePantryItemError>> Handle(UpdatePantryItemCommand command, CancellationToken ct = default);
    Task<Result<Recipe, SuggestRecipeError>> Handle(SuggestRecipeCommand command, CancellationToken ct = default);
    Task<Result<LocationPreference, DetectLocationError>> Handle(DetectLocationCommand command, CancellationToken ct = default);
    Task Handle(UnlockPremiumFeaturesCommand command, CancellationToken ct = default);
    Task Handle(LockPremiumFeaturesCommand command, CancellationToken ct = default);
    Task<Result<LocationPreference, SetHomeCityError>> Handle(SetHomeCityCommand command, CancellationToken ct = default);
    Task<Result<City, ImportCityError>> Handle(ImportCityCommand command, CancellationToken ct = default);
}
