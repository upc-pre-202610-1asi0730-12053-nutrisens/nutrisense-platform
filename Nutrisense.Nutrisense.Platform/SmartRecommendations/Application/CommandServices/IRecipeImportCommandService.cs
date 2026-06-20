using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Errors;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;

public interface IRecipeImportCommandService
{
    /// <summary>Returns the number of new recipes generated and persisted.</summary>
    Task<Result<int, RecipeImportError>> Handle(ImportRecipeSuggestionsCommand command, CancellationToken ct = default);
}
