using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;

public interface IIngredientCatalogImportCommandService
{
    /// <summary>Returns the number of new ingredients derived and persisted.</summary>
    Task<int> Handle(DeriveIngredientsFromFoodsCommand command, CancellationToken ct = default);
}
