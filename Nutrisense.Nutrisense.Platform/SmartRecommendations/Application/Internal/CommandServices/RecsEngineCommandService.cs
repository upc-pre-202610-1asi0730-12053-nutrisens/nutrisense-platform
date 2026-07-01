using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.CommandServices;

public class RecsEngineCommandService(
    IRecommendationCardRepository recommendationCardRepository,
    ILocationPreferenceRepository locationPreferenceRepository,
    IPantryRepository pantryRepository,
    ICityRepository cityRepository,
    IIngredientCatalogRepository ingredientCatalogRepository,
    IRecipeRepository recipeRepository,
    IIamContextFacade iamFacade,
    IBodyHealthMetricsContextFacade bodyHealthMetricsFacade,
    INutritionTrackingContextFacade nutritionTrackingFacade,
    IGeolocationService geolocationService,
    IGeocodingService geocodingService,
    IWeatherService weatherService,
    ILocalFoodSuggestionService localFoodSuggestionService,
    ISubscriptionsContextFacade subscriptionsFacade,
    IMemoryCache cache,
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<RecsEngineCommandService> logger) : IRecsEngineCommandService
{
    public async Task<Result<RecommendationCard, SmartRecommendationsError>> Handle(
        GenerateRecommendationCommand command, CancellationToken ct = default)
    {
        try
        {
            // Resolve the active city: the travel/detected city if set, else the user's home city.
            var locationPref = await locationPreferenceRepository.FindByUserIdAsync(command.UserId, ct);
            var cityId = locationPref?.CurrentCityId ?? locationPref?.HomeCityId;
            if (cityId is null)
            {
                logger.LogInformation(
                    "No location set for user {UserId}; skipping contextual recommendations.", command.UserId);
                return new Result<RecommendationCard, SmartRecommendationsError>.Failure(
                    SmartRecommendationsError.NoFoodsAvailable);
            }

            var city = await cityRepository.FindByIdAsync(cityId.Value, ct);
            if (city is null)
                return new Result<RecommendationCard, SmartRecommendationsError>.Failure(
                    SmartRecommendationsError.NoFoodsAvailable);

            var restrictions = (await iamFacade.GetDietaryRestrictionsByUserId(command.UserId, ct)).ToList();
            var goal = await bodyHealthMetricsFacade.GetActiveGoalByUserId(command.UserId, ct);
            var goalType = (goal?.GoalType ?? "weight-loss").ToLowerInvariant();

            var weather = await weatherService.GetCurrentAsync(cityId.Value, ct);
            var feedSize = configuration.GetValue("Recommendations:FeedSize", 8);

            // Guard: if the user already has active cards generated for this exact context
            // (city + weather bucket + goal), reuse them instead of regenerating. These are the
            // same dimensions that key the suggestion cache, so the result would be identical.
            // This keeps reactive re-detection (same city on every visit) from churning the DB
            // and rotating card IDs needlessly. Restriction changes are handled by their own
            // invalidation path, not here.
            var activeCards = (await recommendationCardRepository.FindActiveListByUserIdAsync(command.UserId, ct)).ToList();
            if (activeCards.Count > 0
                && activeCards.All(c =>
                    c.CityId == cityId
                    && c.WeatherType == weather.WeatherType
                    && c.GoalType == goalType))
            {
                logger.LogInformation(
                    "Reusing {Count} active recommendations for user {UserId} (unchanged context city={CityId}, weather={Weather}, goal={Goal}).",
                    activeCards.Count, command.UserId, cityId, weather.WeatherType, goalType);
                return new Result<RecommendationCard, SmartRecommendationsError>.Success(activeCards[0]);
            }

            // Ask the generator (DeepSeek) for locally-available foods for this city + weather + goal.
            // Cached per context so users sharing it reuse one call (see GetOrFetchSuggestionsAsync).
            var suggestions = await GetOrFetchSuggestionsAsync(city, weather, goalType, restrictions, feedSize, ct);
            if (suggestions.Count == 0)
                return new Result<RecommendationCard, SmartRecommendationsError>.Failure(
                    SmartRecommendationsError.NoFoodsAvailable);

            // Turn the suggested names into real catalog foods (search-first, AI-estimate the rest).
            var provisioned = await nutritionTrackingFacade.ResolveOrCreateFoodsByNames(
                suggestions.Select(s => s.NameEn).ToList(), "ai-local-rec", ct);

            // Defense in depth: never surface a food that conflicts with the user's restrictions.
            var safe = provisioned
                .Where(p => !p.Restrictions.Any(r => restrictions.Contains(r, StringComparer.OrdinalIgnoreCase)))
                .Take(feedSize)
                .ToList();

            if (safe.Count == 0)
                return new Result<RecommendationCard, SmartRecommendationsError>.Failure(
                    SmartRecommendationsError.NoFoodsAvailable);

            await recommendationCardRepository.DeactivateAllByUserIdAsync(command.UserId, ct);

            var labelEn = $"Local pick for {city.NameEn}";
            var labelEs = $"Opción local para {city.NameEs}";

            RecommendationCard? firstCard = null;
            foreach (var p in safe)
            {
                var servingFactor = p.ServingSizeG / 100m;
                var calories = Math.Round(p.CaloriesPer100g * servingFactor, 2);
                var protein = Math.Round(p.ProteinPer100g * servingFactor, 2);
                var carbs = Math.Round(p.CarbsPer100g * servingFactor, 2);
                var fat = Math.Round(p.FatPer100g * servingFactor, 2);
                var badge = protein > 20m ? "high-protein" : calories < 300m ? "light" : "bulk";

                var card = new RecommendationCard(
                    command.UserId,
                    p.NameEn, p.NameEs,
                    calories, protein, carbs, fat,
                    badge, labelEn, labelEs,
                    p.Id, cityId, weather.WeatherType, goalType,
                    p.Restrictions);

                await recommendationCardRepository.AddAsync(card, ct);
                firstCard ??= card;
            }

            await unitOfWork.CompleteAsync(ct);
            await mediator.PublishAsync(new RecommendationGenerated(command.UserId, firstCard!.Id, labelEn));

            return new Result<RecommendationCard, SmartRecommendationsError>.Success(firstCard);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating recommendations for user {UserId}", command.UserId);
            return new Result<RecommendationCard, SmartRecommendationsError>.Failure(
                SmartRecommendationsError.UnexpectedError);
        }
    }

    /// <summary>
    /// Returns the suggested local-food NAMES for a city + weather bucket + goal + restrictions context,
    /// caching them so everyone sharing that context reuses a single DeepSeek call. The weather is already
    /// bucketed (hot/warm/mild/cold), which keeps the cache hit-rate high. Persisted foods dedupe globally
    /// on top of this, so repeat contexts cost neither an LLM call nor a duplicate catalog row.
    /// </summary>
    private async Task<IReadOnlyList<LocalFoodSuggestion>> GetOrFetchSuggestionsAsync(
        City city, WeatherSnapshot weather, string goalType, List<string> restrictions, int count, CancellationToken ct)
    {
        var restrictionsKey = restrictions.Count > 0
            ? string.Join(",", restrictions.OrderBy(r => r, StringComparer.OrdinalIgnoreCase))
            : "none";
        var cacheKey = $"local-food-suggestions:{city.Id}:{weather.WeatherType}:{goalType}:{restrictionsKey}";

        if (cache.TryGetValue(cacheKey, out IReadOnlyList<LocalFoodSuggestion>? cached) && cached is not null)
            return cached;

        var suggestions = await localFoodSuggestionService.SuggestAsync(
            city.NameEn, city.Country, weather.Condition, weather.TempC, weather.WeatherType,
            goalType, restrictions, count, ct);

        if (suggestions.Count > 0)
        {
            var hours = configuration.GetValue("Recommendations:SuggestionCacheHours", 6);
            cache.Set(cacheKey, suggestions, TimeSpan.FromHours(hours));
        }

        return suggestions;
    }

    public async Task<Result<LocationPreference, SmartRecommendationsError>> Handle(
        EnableTravelModeCommand command, CancellationToken ct = default)
    {
        try
        {
            var isPro = await subscriptionsFacade.IsProOrAbove(command.UserId, ct);
            if (!isPro)
                return new Result<LocationPreference, SmartRecommendationsError>.Failure(
                    SmartRecommendationsError.TravelModePlanNotSufficient);

            var city = await cityRepository.FindByIdAsync(command.CurrentCityId, ct);
            if (city is null)
                return new Result<LocationPreference, SmartRecommendationsError>.Failure(
                    SmartRecommendationsError.CityNotFound);

            var locationPref = await locationPreferenceRepository.FindOrCreateAsync(command.UserId, ct);
            locationPref.EnableTravelMode(command.CurrentCityId);
            locationPreferenceRepository.Update(locationPref);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new TravelModeActivated(command.UserId, command.CurrentCityId));
            await Handle(new GenerateRecommendationCommand(command.UserId, "travel-mode"), ct);

            return new Result<LocationPreference, SmartRecommendationsError>.Success(locationPref);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enabling travel mode for user {UserId}", command.UserId);
            return new Result<LocationPreference, SmartRecommendationsError>.Failure(
                SmartRecommendationsError.UnexpectedError);
        }
    }

    public async Task<Result<LocationPreference, SmartRecommendationsError>> Handle(
        DisableTravelModeCommand command, CancellationToken ct = default)
    {
        try
        {
            var locationPref = await locationPreferenceRepository.FindByUserIdAsync(command.UserId, ct);
            if (locationPref is null)
                return new Result<LocationPreference, SmartRecommendationsError>.Failure(
                    SmartRecommendationsError.LocationPreferenceNotFound);

            locationPref.DisableTravelMode();
            locationPreferenceRepository.Update(locationPref);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new TravelModeDeactivated(command.UserId));
            await Handle(new GenerateRecommendationCommand(command.UserId, "travel-disabled"), ct);

            return new Result<LocationPreference, SmartRecommendationsError>.Success(locationPref);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error disabling travel mode for user {UserId}", command.UserId);
            return new Result<LocationPreference, SmartRecommendationsError>.Failure(
                SmartRecommendationsError.UnexpectedError);
        }
    }

    public async Task<Result<Pantry, SmartRecommendationsError>> Handle(
        RegisterPantryItemsCommand command, CancellationToken ct = default)
    {
        try
        {
            var isPro = await subscriptionsFacade.IsProOrAbove(command.UserId, ct);
            if (!isPro)
                return new Result<Pantry, SmartRecommendationsError>.Failure(
                    SmartRecommendationsError.PantryPlanNotSufficient);

            var pantry = await pantryRepository.FindOrCreateAsync(command.UserId, ct);

            foreach (var item in command.Items)
            {
                var ingredient = await ingredientCatalogRepository.FindByIdAsync(item.IngredientCatalogItemId, ct);
                if (ingredient is null)
                    return new Result<Pantry, SmartRecommendationsError>.Failure(
                        SmartRecommendationsError.IngredientNotFound);
            }

            pantry.AddItems(command.Items);
            pantryRepository.Update(pantry);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new PantryUpdated(command.UserId));
            await Handle(new SuggestRecipeCommand(command.UserId), ct);

            return new Result<Pantry, SmartRecommendationsError>.Success(pantry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering pantry items for user {UserId}", command.UserId);
            return new Result<Pantry, SmartRecommendationsError>.Failure(
                SmartRecommendationsError.UnexpectedError);
        }
    }

    public async Task<Result<Pantry, SmartRecommendationsError>> Handle(
        RemovePantryItemCommand command, CancellationToken ct = default)
    {
        try
        {
            var pantry = await pantryRepository.FindByUserIdAsync(command.UserId, ct);
            if (pantry is null)
                return new Result<Pantry, SmartRecommendationsError>.Failure(SmartRecommendationsError.PantryItemNotFound);

            try
            {
                pantry.RemoveItem(command.PantryItemId);
            }
            catch (InvalidOperationException)
            {
                return new Result<Pantry, SmartRecommendationsError>.Failure(SmartRecommendationsError.PantryItemNotFound);
            }

            pantryRepository.Update(pantry);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new PantryUpdated(command.UserId));

            return new Result<Pantry, SmartRecommendationsError>.Success(pantry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing pantry item {ItemId} for user {UserId}",
                command.PantryItemId, command.UserId);
            return new Result<Pantry, SmartRecommendationsError>.Failure(SmartRecommendationsError.UnexpectedError);
        }
    }

    public async Task<Result<Pantry, SmartRecommendationsError>> Handle(
        UpdatePantryItemCommand command, CancellationToken ct = default)
    {
        try
        {
            var pantry = await pantryRepository.FindByUserIdAsync(command.UserId, ct);
            if (pantry is null)
                return new Result<Pantry, SmartRecommendationsError>.Failure(SmartRecommendationsError.PantryItemNotFound);

            try
            {
                pantry.UpdateItem(command.PantryItemId, command.Quantity, command.Unit);
            }
            catch (InvalidOperationException)
            {
                return new Result<Pantry, SmartRecommendationsError>.Failure(SmartRecommendationsError.PantryItemNotFound);
            }

            pantryRepository.Update(pantry);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new PantryUpdated(command.UserId));

            return new Result<Pantry, SmartRecommendationsError>.Success(pantry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating pantry item {ItemId} for user {UserId}",
                command.PantryItemId, command.UserId);
            return new Result<Pantry, SmartRecommendationsError>.Failure(SmartRecommendationsError.UnexpectedError);
        }
    }

    public async Task<Result<Recipe, SmartRecommendationsError>> Handle(
        SuggestRecipeCommand command, CancellationToken ct = default)
    {
        try
        {
            var goal = await bodyHealthMetricsFacade.GetActiveGoalByUserId(command.UserId, ct);
            var goalType = goal?.GoalType ?? "weight-loss";

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var macros = await nutritionTrackingFacade.GetDailyMacroSummary(command.UserId, today, ct);

            var restrictions = (await iamFacade.GetDietaryRestrictionsByUserId(command.UserId, ct)).ToList();

            var recipesForGoal = (await recipeRepository.FindByGoalTypeAsync(goalType, ct)).ToList();
            if (recipesForGoal.Count == 0)
                return new Result<Recipe, SmartRecommendationsError>.Failure(SmartRecommendationsError.NoRecipesAvailable);

            var filtered = recipesForGoal
                .Where(r => !r.RestrictionsConflict.Any(rc => restrictions.Contains(rc)))
                .ToList();

            var recipe = filtered.Count > 0 ? filtered.First() : recipesForGoal.First();

            await mediator.PublishAsync(new RecipeSuggested(command.UserId, recipe.Id));

            return new Result<Recipe, SmartRecommendationsError>.Success(recipe);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error suggesting recipe for user {UserId}", command.UserId);
            return new Result<Recipe, SmartRecommendationsError>.Failure(SmartRecommendationsError.UnexpectedError);
        }
    }

    public async Task<Result<LocationPreference, SmartRecommendationsError>> Handle(
        DetectLocationCommand command, CancellationToken ct = default)
    {
        try
        {
            int cityId;
            var geoResult = await geolocationService.DetectNearestCityAsync(command.Lat, command.Lng, ct);
            if (geoResult.Success && geoResult.NearestCityId is not null)
            {
                // A known city is close enough to the coordinates.
                cityId = geoResult.NearestCityId.Value;
            }
            else
            {
                // No nearby city in the catalog: reverse-geocode and import it.
                var candidate = await geocodingService.ReverseAsync(command.Lat, command.Lng, ct);
                if (candidate is null)
                    return new Result<LocationPreference, SmartRecommendationsError>.Failure(
                        SmartRecommendationsError.DetectionFailed);

                var city = await FindOrImportCityAsync(candidate, ct);
                cityId = city.Id;
            }

            var locationPref = await locationPreferenceRepository.FindOrCreateAsync(command.UserId, ct);
            locationPref.DetectLocation(cityId);
            locationPreferenceRepository.Update(locationPref);
            await unitOfWork.CompleteAsync(ct);

            await mediator.PublishAsync(new LocationDetected(command.UserId, cityId));
            await HandleCheckWeatherAndProfile(new CheckWeatherAndProfileCommand(command.UserId, cityId), ct);

            return new Result<LocationPreference, SmartRecommendationsError>.Success(locationPref);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error detecting location for user {UserId}", command.UserId);
            return new Result<LocationPreference, SmartRecommendationsError>.Failure(
                SmartRecommendationsError.UnexpectedError);
        }
    }

    public async Task Handle(UnlockPremiumFeaturesCommand command, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Premium features are now available for user {UserId} with plan {PlanKey}.",
            command.UserId, command.PlanKey);
        await mediator.PublishAsync(new PremiumFeaturesUnlocked(command.UserId, command.PlanKey));
    }

    public async Task Handle(LockPremiumFeaturesCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Premium features are now locked for user {UserId}.", command.UserId);
        await mediator.PublishAsync(new PremiumFeaturesLocked(command.UserId));
    }

    public async Task<Result<City, SmartRecommendationsError>> Handle(ImportCityCommand command, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(command.Name) || string.IsNullOrWhiteSpace(command.Country))
                return new Result<City, SmartRecommendationsError>.Failure(SmartRecommendationsError.InvalidCityData);

            var candidate = new GeoCityCandidate(
                command.Name, command.NameEn, command.NameEs, command.Country, null, command.Lat, command.Lng);
            var city = await FindOrImportCityAsync(candidate, ct);
            return new Result<City, SmartRecommendationsError>.Success(city);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error importing city '{Name}' ({Country}).", command.Name, command.Country);
            return new Result<City, SmartRecommendationsError>.Failure(SmartRecommendationsError.UnexpectedError);
        }
    }

    /// <summary>
    /// Returns the existing catalog city matching the candidate's natural key, or imports and
    /// persists it. Idempotent; tolerates a concurrent import racing on the unique key.
    /// </summary>
    private async Task<City> FindOrImportCityAsync(GeoCityCandidate candidate, CancellationToken ct)
    {
        var key = City.BuildKey(candidate.Name, candidate.Country, candidate.Lat, candidate.Lng);
        var existing = await cityRepository.FindByKeyAsync(key, ct);
        if (existing is not null) return existing;

        var city = City.Import(
            candidate.Name, candidate.NameEn, candidate.NameEs, candidate.Country, candidate.Lat, candidate.Lng);
        try
        {
            await cityRepository.AddAsync(city, ct);
            await unitOfWork.CompleteAsync(ct);
            return city;
        }
        catch (DbUpdateException)
        {
            var raced = await cityRepository.FindByKeyAsync(key, ct);
            if (raced is not null) return raced;
            throw;
        }
    }

    public async Task<LocationPreference> Handle(SetLocationPermissionCommand command, CancellationToken ct = default)
    {
        var locationPref = await locationPreferenceRepository.FindOrCreateAsync(command.UserId, ct);
        locationPref.SetLocationPermission(command.Granted);
        locationPreferenceRepository.Update(locationPref);
        await unitOfWork.CompleteAsync(ct);
        return locationPref;
    }

    private async Task HandleCheckWeatherAndProfile(CheckWeatherAndProfileCommand command, CancellationToken ct)
    {
        var snapshot = await weatherService.GetCurrentAsync(command.CityId, ct);
        await mediator.PublishAsync(new WeatherContextRetrieved(command.UserId, command.CityId, snapshot.WeatherType));
        await Handle(new GenerateRecommendationCommand(command.UserId, "weather"), ct);
    }

    public async Task<Result<LocationPreference, SmartRecommendationsError>> Handle(SetHomeCityCommand command, CancellationToken ct = default)
    {
        try
        {
            var city = await cityRepository.FindByIdAsync(command.CityId, ct);
            if (city is null)
                return new Result<LocationPreference, SmartRecommendationsError>.Failure(SmartRecommendationsError.CityNotFound);

            var locationPref = await locationPreferenceRepository.FindOrCreateAsync(command.UserId, ct);
            locationPref.SetHomeCity(command.CityId);
            locationPreferenceRepository.Update(locationPref);
            await unitOfWork.CompleteAsync(ct);

            return new Result<LocationPreference, SmartRecommendationsError>.Success(locationPref);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting home city for user {UserId}", command.UserId);
            return new Result<LocationPreference, SmartRecommendationsError>.Failure(SmartRecommendationsError.UnexpectedError);
        }
    }
}
