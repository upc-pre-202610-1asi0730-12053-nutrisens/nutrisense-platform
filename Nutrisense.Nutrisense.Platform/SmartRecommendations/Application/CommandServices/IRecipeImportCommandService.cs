using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Errors;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;

public interface IRecipeImportCommandService
{
    /// <summary>Returns the number of new recipes generated and persisted.</summary>
    Task<Result<int, SmartRecommendationsError>> Handle(ImportRecipeSuggestionsCommand command, CancellationToken ct = default);
}
