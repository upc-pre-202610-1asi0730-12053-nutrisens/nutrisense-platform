# NutriSense Platform

## Project Overview

The `NutriSense Platform` is a robust and scalable backend application designed to power a personalized nutrition, fitness, and wellness experience. It manages food and nutrition tracking, physical activity and wearable integrations, body composition metrics, smart meal recommendations, subscriptions and payments, and analytics reporting. Built with .NET 10, it adheres to Domain-Driven Design (DDD) principles and implements Command Query Responsibility Segregation (CQRS), promoting modularity, maintainability, and testability.

This platform is structured around distinct Bounded Contexts, ensuring clear separation of concerns and enabling independent development and deployment of core functionalities. Cross-context communication is performed exclusively through Anti-Corruption Layer (ACL) facades, keeping each domain model isolated and protected.

## Table of Contents

1.  Architecture Overview
2.  Domain-Driven Design (DDD) Concepts
3.  Key Features & Best Practices Implemented
4.  Bounded Contexts
    *   IAM (Identity and Access Management)
    *   NutritionTracking
    *   ActivityWearable
    *   BodyHealthMetrics
    *   SmartRecommendations
    *   Subscriptions
    *   AnalyticsReporting
5.  External Integrations
6.  Technologies Used
7.  Getting Started
    *   Prerequisites
    *   Setup Instructions
    *   Running with Docker
    *   Configuration & Secrets
8.  Project Structure
9.  License

## Architecture Overview

The project's architecture is driven by **Domain-Driven Design (DDD)** principles and implements **Command Query Responsibility Segregation (CQRS)**. This approach organizes the codebase to align closely with the business domain, separating operations that change state (Commands) from operations that read state (Queries).

*   **Domain Layer**: Contains the core business logic — aggregates, entities, value objects, domain commands/queries, domain events, repository and service interfaces. It is the heart of the application and has no dependencies on other layers.
*   **Application Layer**: Orchestrates domain objects to fulfill use cases. It hosts command and query services (`Internal`), Anti-Corruption Layer facades and outbound services (`Acl`), and domain event handlers. It depends on the Domain layer.
*   **Infrastructure Layer**: Provides implementations for interfaces defined in the Domain and Application layers (e.g., EF Core repositories, JWT/BCrypt services, external API clients, calculators, PDF generation, cross-context lookups). It depends on the Application and Domain layers.
*   **Interfaces Layer (Presentation)**: Handles external communication via REST APIs. It defines controllers, API resources, transform assemblers, and inbound ACL services. It depends on the Application layer.

## Domain-Driven Design (DDD) Concepts

The project embraces DDD to manage complexity across its seven business domains:

*   **Bounded Contexts**: The application is explicitly divided into `IAM`, `NutritionTracking`, `ActivityWearable`, `BodyHealthMetrics`, `SmartRecommendations`, `Subscriptions`, and `AnalyticsReporting`, each with its own ubiquitous language and domain model.
*   **Aggregates**: Key domain objects such as `User`, `NutritionLog`, `Food`, `ActivityLog`, `BodyMetrics`, `Pantry`, `Recipe`, `UserSubscription`, and `UserAnalytics` are defined as aggregate roots that encapsulate clusters of entities and value objects and enforce transactional consistency within their boundaries.
*   **Entities**: Objects with a distinct identity over time (e.g., `City`, `IngredientCatalogItem`, `RecommendationCard`, `PaymentMethod`, `SubscriptionPlan`).
*   **Value Objects**: Immutable objects that measure, quantify, or describe domain concepts (e.g., macronutrient breakdowns, measurement units, money/price amounts), compared by value rather than identity.
*   **Domain Services**: Operations that don't naturally fit within a single entity (e.g., `ITokenService`, `IHashingService`, `IBodyMetricsCalculator`, `ICaloricBalanceCalculator`, `IAdherenceCalculator`, `IRecipeGenerationService`).
*   **Anti-Corruption Layer (ACL)**: Each context exposes a `ContextFacade` and consumes other contexts only through these facades, preventing domain model leakage across boundaries.
*   **Domain Events**: Published via the mediator to coordinate behavior across aggregates and contexts in a loosely coupled way.

## Key Features & Best Practices Implemented

*   **Command Query Responsibility Segregation (CQRS)**: Commands update data and Queries retrieve data, with dedicated command/query services per context.
*   **Result Pattern & Refined Error Management**:
    *   Application services represent success or failure explicitly instead of using exceptions for control flow.
    *   **RFC 7807 Problem Details**: All API error responses adhere to this standard via centralized `AddProblemDetails` configuration and a global exception handler, providing machine-readable, consistent error information without leaking sensitive details.
*   **Internationalization (i18n)**:
    *   `.resx` resources organized per bounded context and in the `Shared` context.
    *   `IStringLocalizer` used for dynamic localized strings.
    *   **Culture Negotiation**: Supported cultures `en`, `en-US`, `es`, `es-PE`, resolved from query string, cookies, or `Accept-Language` headers.
*   **Persistence**: Entity Framework Core with the MySQL provider; pending migrations are applied automatically on startup, and reference data (e.g., subscription plans) is seeded.
*   **Messaging/Mediation**: Cortex.Mediator handles commands and publishes domain events, promoting loose coupling between components.
*   **Authentication & Authorization**: JWT Bearer authentication (HMAC-SHA256) with strict token validation; the JWT secret is validated at startup (minimum length, no unsubstituted placeholders).
*   **Security & Configuration Hygiene**: Secrets (JWT, database, external API keys) are read from environment variables / configuration sections so they never need to be committed.
*   **CORS**: Explicit origin allow-list driven by configuration with credentials enabled to support the SPA frontend's `Authorization: Bearer` header.
*   **Conventions**: Lowercase + kebab-case route naming; snake_case columns and pluralized table names via Humanizer.
*   **Background Processing**: A hosted service performs gated catalog import (USDA foods, DeepSeek enrichment, derived ingredients, and generated recipes) after startup so the app stays responsive.
*   **API Documentation**: Swagger / OpenAPI (Swashbuckle with annotations) enabled in all environments to support onboarding and exploratory testing.

## Bounded Contexts

### IAM (Identity and Access Management)

Responsible for user identity, authentication, and access control across the platform.

*   **Scope**: Registration, sign-in, JWT issuance/validation, session management, and user queries.
*   **Aggregates**: `User`, `UserAudit`
*   **Domain Services**: `ITokenService` (JWT), `IHashingService` (BCrypt)
*   **API Endpoints**: `/api/v1/authentication`, `/api/v1/users`, `/api/v1/users/{userId}/sessions`

### NutritionTracking

Manages the catalog of foods and the user's daily nutrition logging, including AI-assisted estimation.

*   **Scope**: Food catalog management, nutrition logging, dish/menu image analysis, nutrition estimation, and dietary restriction checks.
*   **Aggregates**: `Food`, `FoodAudit`, `NutritionLog`, `NutritionLogAudit`
*   **Domain Services**: `IDishVisionService`, `IMenuVisionService`, `IFoodNutritionEstimationService`, `IFoodSearchService`, `IFoodRestrictionChecker`, `IFoodEnrichmentService`
*   **API Endpoints**: `/api/v1/foods`, `/api/v1/nutrition-logs`

### ActivityWearable

Tracks physical activity and integrates with wearable devices.

*   **Scope**: Activity logging, wearable connections/sync, and caloric balance calculation.
*   **Aggregates**: `ActivityLog`, `ActivityLogAudit`, `WearableConnection`, `WearableConnectionAudit`
*   **Domain Services**: `ICaloricBalanceCalculator`, `IWearableSyncProvider` (Google Fit)
*   **API Endpoints**: `/api/v1/activity-logs`, `/api/v1/wearable-connections`

### BodyHealthMetrics

Manages body composition and health metrics derived from user data.

*   **Scope**: Body metrics tracking and computation (e.g., BMR/TDEE via the Mifflin–St Jeor formula).
*   **Aggregates**: `BodyMetrics`
*   **Domain Services**: `IBodyMetricsCalculator` (Mifflin–St Jeor)
*   **API Endpoints**: `/api/v1/body-metrics`

### SmartRecommendations

Generates personalized meal and ingredient recommendations based on the user's pantry, location, and weather.

*   **Scope**: Pantry management, ingredient catalog, recipes, location preferences, weather/geolocation-aware suggestions, and a recommendation engine (subscription-tier aware).
*   **Aggregates/Entities**: `Pantry`, `Recipe`, `City`, `IngredientCatalogItem`, `LocationPreference`, `RecommendationCard` (plus audit entities)
*   **Domain Services**: `IGeolocationService`, `IWeatherService`, `IGeocodingService`, `IRecipeGenerationService`, `ILocalFoodSuggestionService`, `ISubscriptionTierLookup`
*   **API Endpoints**: `/api/v1/pantry`, `/api/v1/recipes`, `/api/v1/cities`, `/api/v1/ingredient-catalog`, `/api/v1/location-preferences`, `/api/v1/recommendations`

### Subscriptions

Handles subscription plans, user subscriptions, payment methods, and payment processing.

*   **Scope**: Plan catalog, subscription lifecycle, payment methods, and payment processing via a payment gateway.
*   **Aggregates/Entities**: `SubscriptionPlan`, `UserSubscription`, `PaymentMethod` (plus audit entities)
*   **Domain Services**: `IPaymentGateway` (Stripe)
*   **API Endpoints**: `/api/v1/subscription-plans`, `/api/v1/user-subscriptions`, `/api/v1/payment-methods`, `/api/v1/payments`

### AnalyticsReporting

Aggregates user data into analytics and exportable reports.

*   **Scope**: Adherence and streak analytics, user analytics aggregation, and PDF report generation.
*   **Aggregates**: `UserAnalytics`, `UserAnalyticsAudit`
*   **Domain Services**: `IAdherenceCalculator`, `IStreakCalculator`
*   **API Endpoints**: `/api/v1/analytics`

## External Integrations

The platform integrates with several third-party services, all configured via API keys supplied through environment variables:

*   **USDA FoodData Central** — authoritative food/nutrition data for the food catalog.
*   **DeepSeek** — AI food nutrition estimation, enrichment, recipe and local-food suggestion generation.
*   **Google Gemini** — vision-based dish and menu image analysis.
*   **OpenWeatherMap** — weather and geocoding for location-aware recommendations.
*   **Stripe** — payment processing for subscriptions.
*   **Google Fit** — wearable activity synchronization.

## Technologies Used

*   **.NET 10** — core framework.
*   **ASP.NET Core** — RESTful APIs.
*   **Entity Framework Core 10** (`MySql.EntityFrameworkCore`) — ORM over MySQL.
*   **MySQL** — relational database.
*   **Cortex.Mediator** — mediator pattern for commands and domain events.
*   **BCrypt.Net-Next** — secure password hashing.
*   **Microsoft.AspNetCore.Authentication.JwtBearer** — JWT authentication.
*   **Swashbuckle.AspNetCore (+ Annotations)** — OpenAPI/Swagger documentation.
*   **Microsoft.Extensions.Localization** — internationalization (i18n).
*   **Humanizer** — snake_case columns and pluralized table names.

## Getting Started

### Prerequisites

*   .NET 10 SDK
*   MySQL Server (or Docker for local development)
*   Git

### Setup Instructions

1.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    cd nutrisense-complete
    ```

2.  **Navigate to the project directory:**
    ```bash
    cd Nutrisense.Nutrisense.Platform
    ```

3.  **Restore NuGet packages:**
    ```bash
    dotnet restore
    ```

4.  **Configure the database:**
    *   Ensure your MySQL server is running.
    *   Update the `DefaultConnection` string in `appsettings.json` (and `appsettings.Development.json`) to point to your MySQL instance.
    *   On startup the application applies pending EF Core migrations automatically and seeds reference data (e.g., subscription plans).

5.  **Configure secrets and API keys** (see [Configuration & Secrets](#configuration--secrets)).

6.  **Run the application:**
    ```bash
    dotnet run
    ```

7.  **Access Swagger UI:**
    Navigate to the app's base URL `/swagger` to explore the API endpoints.

### Running with Docker

A `Dockerfile` and `docker-compose.yml` are provided. The container exposes port `8080`.

```bash
docker compose up --build
```

The compose file injects the connection string, JWT secret, and external API keys from environment variables (`DATABASE_URL`, `DATABASE_USER`, `DATABASE_PASSWORD`, `DATABASE_SCHEMA`, `JWT_SECRET`, `OPENWEATHERMAP_API_KEY`, `DEEPSEEK_API_KEY`, `GEMINI_API_KEY`, `USDA_API_KEY`).

### Configuration & Secrets

Secrets are read from configuration sections / environment variables and should not be committed:

*   `TokenSettings:Secret` (env `JWT_SECRET`) — JWT signing key. **Must be at least 32 characters**; the app refuses to start with an unsubstituted placeholder.
*   `ConnectionStrings:DefaultConnection` — MySQL connection string (supports environment-variable expansion).
*   `OpenWeatherMap:ApiKey`, `Usda:ApiKey`, `DeepSeek:ApiKey`, `Gemini:ApiKey` — external integration keys.
*   `Cors:AllowedOrigins` — allowed frontend origins.
*   `Seeder:Enabled` — toggles the background catalog import (USDA foods, DeepSeek enrichment, ingredients, recipes).

## Project Structure

The project is organized by **Bounded Contexts** at the top level, with each context further decomposed into architectural layers:

```
nutrisense-complete/
├── Nutrisense.Nutrisense.Platform/
│   ├── IAM/                       # Identity & Access Management BC
│   │   ├── Application/           # Command/Query services, ACL facades, handlers
│   │   ├── Domain/                # Aggregates, Entities, Value Objects, interfaces
│   │   ├── Infrastructure/        # EF Core repositories, JWT, BCrypt
│   │   └── Interfaces/            # REST controllers, resources, assemblers, ACL
│   ├── NutritionTracking/         # Foods & nutrition logging BC
│   ├── ActivityWearable/          # Activity & wearable integration BC
│   ├── BodyHealthMetrics/         # Body composition metrics BC
│   ├── SmartRecommendations/      # Pantry, recipes & recommendations BC
│   ├── Subscriptions/             # Plans, subscriptions & payments BC
│   ├── AnalyticsReporting/        # Analytics & PDF reporting BC
│   ├── Shared/                    # Cross-cutting concerns
│   │   ├── Application/           # Common application models
│   │   ├── Domain/                # Base Repository / Unit of Work interfaces
│   │   ├── Infrastructure/        # AppDbContext, base repositories, seeding, external clients
│   │   └── Resources/             # Shared localization files
│   ├── Migrations/                # EF Core migrations
│   ├── Resources/                 # Shared resource entry point
│   ├── Program.cs                 # Application startup & DI configuration
│   ├── appsettings.json           # Configuration files
│   └── Nutrisense.Nutrisense.Platform.csproj
├── nutrisense-platform.sln
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## License

This project is licensed under the **MIT License**. See the [`LICENSE.md`](LICENSE.md) file for details.
