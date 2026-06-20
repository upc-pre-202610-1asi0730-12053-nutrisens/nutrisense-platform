using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Repositories;

public class RecipeRepository(AppDbContext context)
    : BaseRepository<Recipe>(context), IRecipeRepository
{
    private IQueryable<Recipe> WithIngredients() =>
        Context.Set<Recipe>().Include(r => r.Ingredients);

    public async Task<IEnumerable<Recipe>> FindByGoalTypeAsync(string goalType, CancellationToken ct = default) =>
        await WithIngredients().Where(r => r.GoalType == goalType).ToListAsync(ct);

    Task<Recipe?> IBaseRepository<Recipe>.FindByIdAsync(int id, CancellationToken ct) =>
        WithIngredients().FirstOrDefaultAsync(r => r.Id == id, ct);
}
