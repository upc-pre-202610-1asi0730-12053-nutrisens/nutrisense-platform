using System.Text;
using Cortex.Mediator.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
// Shared
using Nutrisense.Nutrisense.Platform.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
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

var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
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
