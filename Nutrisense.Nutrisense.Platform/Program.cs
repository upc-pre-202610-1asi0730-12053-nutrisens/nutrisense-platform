using System.Text;
using Cortex.Mediator.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
// IAM BC
using Nutrisense.Nutrisense.Platform.IAM.Application.Acl;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.IAM.Application.Internal.CommandServices;
using Nutrisense.Nutrisense.Platform.IAM.Application.Internal.QueryServices;
using Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.IAM.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Services;
using Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Mailing;
using Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Hashing.BCrypt;
using Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Tokens.JWT;
// BodyHealthMetrics BC
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Acl;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Internal.CommandServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Internal.QueryServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Services;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Infrastructure.Calculators;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Infrastructure.Persistence.EFC.Repositories;
// Subscriptions BC
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.Acl;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.Internal.CommandServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.Internal.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Services;
using Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.External.Stripe;
using Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.CrossContext;
using Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Seeders;
// NutritionTracking BC
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Acl;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.DeepSeek;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.Gemini;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.Usda;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Services;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.DeepSeek;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.Gemini;
// ActivityWearable BC
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Acl;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Internal.CommandServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Internal.QueryServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Services;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.Calculators;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.External.GoogleHealth;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.Persistence.EFC.Repositories;
// SmartRecommendations BC
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External.DeepSeek;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Repositories;
// SmartRecommendations domain services — aliased to avoid clash with other BCs' domain services
using SmartRecsServices = Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;
// AnalyticsReporting BC
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal.CommandServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal.QueryServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Infrastructure.Calculators;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Infrastructure.Persistence.EFC.Repositories;
// AnalyticsReporting domain services — aliased to avoid clash with SmartRecommendations
using AnalyticsServices = Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Services;
// Shared
using Nutrisense.Nutrisense.Platform.Shared.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Seeding;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Interfaces.ASP.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Lower Case URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Localization Configuration
builder.Services.AddLocalization();

// Configure Kebab Case Route Naming Convention
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

// Register RFC 7807 ProblemDetails payloads for centralized exception handling.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails.Status is null or >= 500)
        {
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
            context.ProblemDetails.Title ??= localizer["UnexpectedServerError"].Value;
            context.ProblemDetails.Detail ??= localizer["UnexpectedErrorProcessingRequest"].Value;
        }
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.CustomSchemaIds(type => type.FullName!.Replace("+", "."));
});

// Configure Database Context and route EF logs through the app logger pipeline.
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// CORS Configuration
// Allowed origins are read from the "Cors:AllowedOrigins" section so dev/prod
// origins can change without recompiling. AllowCredentials is enabled because
// the frontend sends an "Authorization: Bearer {token}" header (JWT); note that
// AllowCredentials is incompatible with AllowAnyOrigin, hence explicit WithOrigins.
const string frontendCorsPolicy = "FrontendPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// JWT Bearer Authentication
// Validation parameters mirror JwtTokenService exactly: HMAC-SHA256, same issuer/audience, short clock skew.
var tokenSettings = builder.Configuration.GetSection("TokenSettings");
var jwtSecret = tokenSettings["Secret"]
    ?? throw new InvalidOperationException("TokenSettings:Secret is not configured.");
if (jwtSecret.Contains('%'))
    throw new InvalidOperationException("TokenSettings:Secret contains an unsubstituted placeholder. Set the JWT_SECRET environment variable.");
if (jwtSecret.Length < 32)
    throw new InvalidOperationException("TokenSettings:Secret must be at least 32 characters long.");
var jwtIssuer = tokenSettings["Issuer"] ?? "nutrisense-platform";
var jwtAudience = tokenSettings["Audience"] ?? "nutrisense-clients";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/problem+json";

                var localizer = context.HttpContext.RequestServices
                    .GetRequiredService<IStringLocalizer<SharedResource>>();

                var problem = new
                {
                    type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    title = localizer["Unauthorized"].Value,
                    status = 401,
                    detail = localizer["AuthenticationRequired"].Value,
                    instance = context.Request.Path.Value
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        };
    });

// Event bus — scans all INotificationHandler<T> in this assembly
builder.Services.AddCortexMediator(new[] { typeof(Program) });

// Shared Bounded Context Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IAM Bounded Context
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IHashingService, BCryptHashingService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// BodyHealthMetrics Bounded Context
builder.Services.AddScoped<IBodyMetricsRepository, BodyMetricsRepository>();
builder.Services.AddScoped<IBodyMetricsCalculator, MifflinStJeorBodyMetricsCalculator>();
builder.Services.AddScoped<IBodyMetricsCommandService, BodyMetricsCommandService>();
builder.Services.AddScoped<IBodyMetricsQueryService, BodyMetricsQueryService>();
builder.Services.AddScoped<IBodyHealthMetricsContextFacade, BodyHealthMetricsContextFacade>();

// Subscriptions Bounded Context
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
builder.Services.AddScoped<IPaymentGateway, StripePaymentGateway>();
builder.Services.AddScoped<IUserSubscriptionCommandService, UserSubscriptionCommandService>();
builder.Services.AddScoped<IUserSubscriptionQueryService, UserSubscriptionQueryService>();
builder.Services.AddScoped<IPaymentMethodCommandService, PaymentMethodCommandService>();
builder.Services.AddScoped<IPaymentMethodQueryService, PaymentMethodQueryService>();
builder.Services.AddScoped<ISubscriptionPlanQueryService, SubscriptionPlanQueryService>();
builder.Services.AddScoped<ISubscriptionsContextFacade, SubscriptionsContextFacade>();
builder.Services.AddScoped<ISubscriptionTierLookup, CrossContextSubscriptionTierLookup>();

// NutritionTracking Bounded Context
builder.Services.AddScoped<INutritionLogRepository, NutritionLogRepository>();
builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IDishVisionService, GeminiDishVisionService>();
builder.Services.AddScoped<IMenuVisionService, GeminiMenuVisionService>();
builder.Services.AddScoped<IFoodNutritionEstimationService, DeepSeekFoodEstimationService>();
builder.Services.AddScoped<IFoodSearchService, DbFoodSearchService>();
builder.Services.AddScoped<IFoodRestrictionChecker, FoodRestrictionChecker>();
builder.Services.AddScoped<INutritionLogCommandService, NutritionLogCommandService>();
builder.Services.AddScoped<INutritionLogQueryService, NutritionLogQueryService>();
builder.Services.AddScoped<IFoodCommandService, FoodCommandService>();
builder.Services.AddScoped<IFoodProvisioningService, FoodProvisioningService>();
builder.Services.AddScoped<IFoodQueryService, FoodQueryService>();
builder.Services.AddScoped<IFoodImportCommandService, FoodImportCommandService>();
builder.Services.AddScoped<IFoodEnrichmentService, DeepSeekFoodEnrichmentService>();
builder.Services.AddScoped<INutritionTrackingContextFacade, NutritionTrackingContextFacade>();
builder.Services.AddHttpClient<IExternalFoodDataProvider, UsdaFoodDataProvider>();
builder.Services.AddHttpClient<DeepSeekClient>();
builder.Services.AddHttpClient<GeminiClient>();

// ActivityWearable Bounded Context
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<IWearableConnectionRepository, WearableConnectionRepository>();
builder.Services.AddScoped<ICaloricBalanceCalculator, CaloricBalanceCalculator>();
builder.Services.AddScoped<IActiveCalorieEstimator, MetActiveCalorieEstimator>();
builder.Services.AddHttpClient<IWearableSyncProvider, GoogleHealthSyncProvider>();
builder.Services.AddScoped<IActivityLogCommandService, ActivityLogCommandService>();
builder.Services.AddScoped<IActivityLogQueryService, ActivityLogQueryService>();
builder.Services.AddScoped<IWearableConnectionCommandService, WearableConnectionCommandService>();
builder.Services.AddScoped<IWearableConnectionQueryService, WearableConnectionQueryService>();
builder.Services.AddScoped<IActivityWearableContextFacade, ActivityWearableContextFacade>();

// SmartRecommendations Bounded Context
builder.Services.AddScoped<IPantryRepository, PantryRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IIngredientCatalogRepository, IngredientCatalogRepository>();
builder.Services.AddScoped<ILocationPreferenceRepository, LocationPreferenceRepository>();
builder.Services.AddScoped<IRecommendationCardRepository, RecommendationCardRepository>();
builder.Services.AddScoped<SmartRecsServices.IGeolocationService, GeolocationService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<SmartRecsServices.IWeatherService, OpenWeatherMapWeatherService>();
builder.Services.AddHttpClient<SmartRecsServices.IGeocodingService, OpenWeatherMapGeocodingService>();
builder.Services.AddScoped<SmartRecsServices.IRecipeGenerationService, DeepSeekRecipeGenerationService>();
builder.Services.AddScoped<SmartRecsServices.ILocalFoodSuggestionService, DeepSeekLocalFoodSuggestionService>();
builder.Services.AddScoped<IIngredientCatalogImportCommandService, IngredientCatalogImportCommandService>();
builder.Services.AddScoped<IRecipeImportCommandService, RecipeImportCommandService>();
builder.Services.AddScoped<IRecsEngineCommandService, RecsEngineCommandService>();
builder.Services.AddScoped<IRecsEngineQueryService, RecsEngineQueryService>();

// AnalyticsReporting Bounded Context
builder.Services.AddScoped<IUserAnalyticsRepository, UserAnalyticsRepository>();
builder.Services.AddScoped<AnalyticsServices.IAdherenceCalculator, AdherenceCalculator>();
builder.Services.AddScoped<AnalyticsServices.IStreakCalculator, StreakCalculator>();
builder.Services.AddScoped<IAnalyticsCommandService, AnalyticsCommandService>();
builder.Services.AddScoped<IAnalyticsQueryService, AnalyticsQueryService>();

// Background catalog import (USDA foods + DeepSeek enrichment, derived ingredients, generated recipes).
// Gated by Seeder:Enabled and per-stage count thresholds; runs after startup so the app stays responsive.
builder.Services.AddHostedService<CatalogImportHostedService>();

var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    await new SubscriptionPlanSeeder(context).SeedAsync();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Swagger UI is intentionally enabled in all environments (including production)
// to support learning, onboarding, and exploratory testing of this API.
app.UseSwagger();
app.UseSwaggerUI();

// Localization Configuration
string[] supportedCultures = ["en", "en-US", "es", "es-PE"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS must run before authentication/authorization so preflight (OPTIONS)
// requests and the Authorization header survive the pipeline.
app.UseCors(frontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
