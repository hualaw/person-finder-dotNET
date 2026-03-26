# PersonFinderDotNET System Design (DDD + Clean Layers)

## Goals
- Keep business rules independent from frameworks.
- Keep application use-cases explicit and testable.
- Allow infrastructure swaps (in-memory -> EF Core/PostgreSQL, messaging, cache) without touching domain logic.
- Scale teams by bounded contexts and vertical slices.

## Layer Model
- Domain (`src/PersonFinder.Domain`)
: Entities, value objects, domain events, domain services, repository contracts.
- Application (`src/PersonFinder.Application`)
: Use-case orchestration, commands/queries, DTOs, validation, behavior pipelines.
- Infrastructure (`src/PersonFinder.Infrastructure`)
: Persistence implementation, repositories, external services, messaging adapters.
- API (`src/PersonFinder.API`)
: HTTP transport concerns only: controllers/contracts/middleware/openapi.

## Dependency Rules
- `Domain` depends on nothing.
- `Application` depends on `Domain`.
- `Infrastructure` depends on `Application` and `Domain`.
- `API` depends on `Application` and `Infrastructure`.

## High-Level Request Flow
1. API receives request and maps transport contract to command/query.
2. Application handler validates and orchestrates use-case.
3. Domain model enforces business invariants.
4. Infrastructure persists and integrates with external systems.
5. API returns response contract.

## Directory Blueprint
```text
PersonFinderDotNET/
  src/
    PersonFinder.Domain/
      Common/
      Entities/
      ValueObjects/
      Events/
      Repositories/
      Services/
      Specifications/
    PersonFinder.Application/
      Abstractions/
      Behaviors/
      Common/
      DTOs/
      Features/
        Persons/
          Commands/
          Queries/
          EventHandlers/
      Mappings/
      Validators/
    PersonFinder.Infrastructure/
      DependencyInjection/
      Persistence/
        Configurations/
        Repositories/
      Services/
      Messaging/
    PersonFinder.API/
      Contracts/
        Requests/
        Responses/
      Controllers/
      Middleware/
      Extensions/
      OpenApi/
  tests/
    PersonFinder.Domain.Tests/
    PersonFinder.Application.Tests/
    PersonFinder.Infrastructure.Tests/
    PersonFinder.API.Tests/
  docs/
    architecture/
      system-design.md
```

## Scalability Notes
- Split `Features/*` by bounded context when the domain grows.
- Use outbox/event bus behind `Infrastructure/Messaging` to scale async workflows.
- Keep read-heavy queries isolated and optimize independently (CQRS style).
- Add caching in infrastructure without changing application handlers.
