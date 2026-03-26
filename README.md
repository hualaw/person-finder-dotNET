# PersonFinderDotNET

DDD-oriented .NET solution scaffold with clean layering and scalable boundaries.

## Projects
- `src/PersonFinder.Domain`: Domain model and business rules.
- `src/PersonFinder.Application`: Use-cases (commands/queries) and abstractions.
- `src/PersonFinder.Infrastructure`: Persistence and external integrations.
- `src/PersonFinder.API`: ASP.NET Core transport layer.
- `tests/*`: Layer-aligned test projects.

## Quick Start
```bash
cp src/PersonFinder.API/appsettings.json.sample src/PersonFinder.API/appsettings.json
dotnet restore PersonFinderDotNET.slnx
dotnet build PersonFinderDotNET.slnx
dotnet test PersonFinderDotNET.slnx
dotnet run --project src/PersonFinder.API
```

Before running the API, update `src/PersonFinder.API/appsettings.json` with your local database and Gemini settings.

## Swagger UI

After running the API, access the interactive API documentation at:
- http://localhost:5265/swagger (HTTP)
- https://localhost:7260/swagger (HTTPS)

The Swagger UI allows you to:
- View all available endpoints
- Read request/response schemas
- Test API calls directly from the browser

## Unit Tests

Run all tests in the solution:
```bash
dotnet test PersonFinderDotNET.slnx
```

Run tests for a specific layer:
```bash
# Domain layer tests (27 tests)
dotnet test tests/PersonFinder.Domain.Tests/PersonFinder.Domain.Tests.csproj

# Application layer tests (40 tests)
dotnet test tests/PersonFinder.Application.Tests/PersonFinder.Application.Tests.csproj

# Infrastructure layer tests
dotnet test tests/PersonFinder.Infrastructure.Tests/PersonFinder.Infrastructure.Tests.csproj

# API layer tests
dotnet test tests/PersonFinder.API.Tests/PersonFinder.API.Tests.csproj
```

Run tests with code coverage:
```bash
dotnet test PersonFinderDotNET.slnx /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

### Test Coverage by Layer

- **Domain**: Entity aggregate root, value object validation, business rule enforcement
- **Application**: Command/query handler logic, input validation, security concerns
- **Infrastructure**: Repository patterns, persistence layer (placeholder)
- **API**: Controller endpoints, request/response mapping (placeholder)

## System Design
- See `docs/architecture/system-design.md`.
