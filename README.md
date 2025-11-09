# User Management API

## Overview
The User Management API is an ASP.NET Core service that exposes RESTful endpoints for creating, reading, updating, and deleting user resources. The application is designed with production-ready concerns in mind, including consistent logging, robust validation, automated documentation, and a relational data store powered by Entity Framework Core.

## Features
- CRUD endpoints for managing user accounts and profile data
- ASP.NET Core minimal hosting model with REST best practices
- Entity Framework Core data access over PostgreSQL (default) with migrations
- Centralized validation pipeline for request DTOs and domain invariants
- Structured logging with middleware to enrich telemetry and correlate requests
- OpenAPI (Swagger) discovery, interactive docs, and client generation support
- Docker compose definition for local infrastructure services (database, API)

## Technology Stack
- .NET 9 SDK
- ASP.NET Core Web API
- Entity Framework Core with Npgsql provider
- FluentValidation (planned) for request and domain validation
- Serilog (planned) for structured logging
- Swashbuckle / NSwag for OpenAPI and TypeScript client generation
- Docker and Docker Compose for containerized development
- PostgreSQL 16+ for persistence

## Getting Started
### Prerequisites
- .NET 9 SDK installed (`dotnet --version`)
- Docker Desktop (for running PostgreSQL locally)
- Node.js 18+ and npm (for optional TypeScript client generation)

### Clone and Restore
```bash
# clone the repository
 git clone <repository-url>
 cd UserManagement

# restore NuGet dependencies
 dotnet restore
```

### Local Environment Variables
The application expects a PostgreSQL connection string. During development you can rely on the default `POSTGRES_URI` defined in `Program.cs`, or supply values via `appsettings.Development.json` or the environment:

```nu
$env.ConnectionStrings__UserDatabase = "Host=localhost;Port=5432;Database=user_db;Username=myuser;Password=mypassword"
```

> Tip: When running in Docker Compose, the service uses the `db` hostname defined in `docker-compose.yml`.

### Run the API Locally
```bash
# run directly with the .NET CLI
dotnet run --project UserManagement.csproj
```
The API listens on `https://localhost:5001` by default. Swagger UI becomes available at `/swagger` when running in Development.

### Run with Docker Compose
```bash
# build and start the api plus postgres
docker compose up --build
```
This command provisions the `db` PostgreSQL instance and the API container. Inspect logs with `docker compose logs -f api`.

## Database and Entity Framework Core
- Add or update migrations with:
  ```bash
  dotnet ef migrations add <MigrationName>
  dotnet ef database update
  ```
- Configure the EF Core DbContext, entity configurations, and seeding in `Controllers/` (or a dedicated `Infrastructure/` folder as the project grows).
- For repeatable local environments, seed reference data in a hosted service or migration script.

## Validation Strategy
- Introduce request DTOs separate from persistence models.
- Wire up FluentValidation (or ASP.NET Core validation attributes) inside the dependency injection container.
- Use middleware or endpoint filters to ensure requests that fail validation return RFC7807-problem responses with actionable error messages.

## Logging and Telemetry
- Add a logging middleware (e.g., Serilog request logging) to enrich log events with correlation IDs, status codes, and execution times.
- Forward logs to stdout by default so container orchestrators can capture them.
- Consider configuring application insights or OpenTelemetry exporters for distributed tracing.

## OpenAPI and Swagger
- Enable Swashbuckle/NSwag services in `Program.cs` via `AddEndpointsApiExplorer()` and `AddSwaggerGen()` or `AddOpenApi()`.
- Host Swagger UI in Development to surface live API contracts.
- Export the OpenAPI document after build:
  ```bash
  dotnet build
  dotnet swagger tofile --output ./swagger.json bin/Debug/net9.0/UserManagement.dll v1
  ```

### Seed Data
- On application startup the `Data/DatabaseSeeder.cs` helper inserts five representative users the first time the database is empty.
- The seeded records cover active/inactive scenarios and a few global regions so the UI has meaningful data immediately.
- Running against an existing database leaves your data untouched; the seeder only executes when no user rows exist.

### REST Request Examples
- The `UserManagement.http` scratch file now targets the Users API with ready-to-run GET/POST/PUT/DELETE examples.
- Adjust the `@baseAddress` and `@userId` variables at the top of the file to suit your local environment.
- Alternatively translate the examples into `curl` or your preferred API client.

### TypeScript Client Generation
- Restore the local `dotnet` tools manifest (installs `swashbuckle.aspnetcore.cli`):
  ```bash
  dotnet tool restore
  ```
- Generate the OpenAPI document and a TypeScript Fetch client in one step:
  ```bash
  nu scripts/generate-typescript-client.nu
  ```
- The script emits `artifacts/swagger.json` and writes the generated client into `clients/typescript` (overwriting the folder each run).
- Install [`npx`](https://docs.npmjs.com/cli/v10/commands/npx) or have `@openapitools/openapi-generator-cli` globally available; the script shells out to it under the hood.
- Treat the generated client as disposable build output unless you explicitly want it source-controlled.

## Testing
- Prepare unit tests under a `tests/` directory using xUnit or NUnit.
- Cover domain logic, validation rules, and any custom middleware.
- For integration testing, spin up a test PostgreSQL instance (Docker) and run tests through `dotnet test` with an `ASPNETCORE_ENVIRONMENT=Test` configuration.

## Project Structure (Suggested Evolution)
```
UserManagement/
├── Controllers/           # API endpoints
├── Domain/                # Entities, value objects, business rules
├── Application/           # Services, DTOs, validation logic
├── Infrastructure/        # EF Core context, repositories, migrations
├── Middleware/            # Logging, exception handling, validation
├── Tests/                 # Unit and integration tests
└── README.md
```

As features mature, reorganize the project toward a clean or onion architecture with explicit boundaries between layers.

## CI/CD Considerations
- Validate commits with `dotnet format`, `dotnet test`, and static analyzers like `dotnet sonarscanner`.
- Bundle Docker images tagged with the commit SHA for traceability.
- Publish the OpenAPI document as a pipeline artifact to automate downstream client generation.

## Next Steps
1. Scaffold the EF Core DbContext and initial migration.
2. Add user CRUD endpoints with validation and logging.
3. Configure Swagger/NSwag services in `Program.cs`.
4. Set up xUnit test projects and baseline coverage.
5. Automate client generation from OpenAPI in CI.
