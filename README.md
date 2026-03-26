# Person Finder (.NET)

## Overview

This project is a production-style backend service built with ASP.NET Core, designed to support geo-spatial search at scale.

It was rebuilt from a Kotlin-based implementation into .NET to demonstrate cross-stack engineering capability and system design skills.

The system enables efficient "nearby user" queries using PostGIS and follows clean architecture principles to ensure maintainability and scalability.

---

## Architecture

The project follows Clean Architecture:

API → Application → Domain → Infrastructure

* **API**: Handles HTTP requests and responses
* **Application**: Contains use cases and business logic
* **Domain**: Core business models
* **Infrastructure**: Database access and external dependencies

---

## Key Features

* Geo-spatial search using PostGIS (ST_DWithin)
* RESTful API design with pagination support
* Clean architecture with clear separation of concerns
* Dependency injection and modular design
* Dockerised PostgreSQL with PostGIS

---

## Tech Stack

* ASP.NET Core 8
* EF Core
* PostgreSQL + PostGIS
* NetTopologySuite
* Docker

---

## Example API

GET /api/v1/persons/nearby?lat=-36.8485&lng=174.7633&radius=1000&page=1&pageSize=20

---

## Design Highlights

* Used PostGIS instead of in-memory distance calculation for performance and scalability
* Applied repository pattern to decouple business logic from data access
* Structured the system to support future scaling (e.g., caching, horizontal scaling)

---

## How to Run

```bash
docker-compose up -d
dotnet run
```

---

## Future Improvements

* Add caching layer (Redis)
* Introduce authentication and authorization
* Implement rate limiting
* Add monitoring and logging (e.g., Serilog + OpenTelemetry)

---
