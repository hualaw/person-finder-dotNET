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

## System Design
- See `docs/architecture/system-design.md`.
