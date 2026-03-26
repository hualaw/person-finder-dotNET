# System Design - Person Finder

## 1. Problem Definition

Design a backend service that allows users to find nearby people based on geographic coordinates.

Requirements:

* Support geo-spatial queries within a given radius
* Handle increasing user volume efficiently
* Ensure low-latency responses

---

## 2. High-Level Architecture

Client → API → Application → Infrastructure → PostgreSQL (PostGIS)

---

## 3. Key Design Decisions

### 3.1 Why PostGIS?

Instead of calculating distances in application code, PostGIS is used to:

* Leverage spatial indexing (GIST)
* Improve query performance
* Enable scalable geo queries

---

### 3.2 Why Clean Architecture?

* Decouples business logic from infrastructure
* Improves testability
* Allows easier technology changes

---

### 3.3 API Design

* RESTful endpoints
* Supports pagination
* Designed for future extension (filters, sorting)

---

## 4. Data Model

Users:

* id (UUID)
* name
* location (GEOGRAPHY Point)

---

## 5. Performance Considerations

* Use ST_DWithin for efficient radius queries
* Add GIST index on location column
* Avoid application-level distance calculations

---

## 6. Scalability

Future scaling strategies:

* Add Redis caching for frequent queries
* Horizontal scaling via stateless API
* Database read replicas

---

## Trade-offs

- Using PostGIS introduces operational complexity but significantly improves query performance compared to application-level calculations  
- Clean architecture increases initial development overhead but enables better scalability and maintainability in the long term  

---

## 8. Failure Handling

* Handle invalid coordinates
* Gracefully manage database connectivity issues
* Support retry mechanisms for transient failures

---

## Request Flow

1. Client sends request with lat/lng and radius  
2. API layer validates input  
3. Application layer processes the use case  
4. Repository queries PostGIS using spatial functions  
5. Results are transformed into DTOs and returned  

## Future Scalability

The system is designed to support:

- Horizontal scaling of API services  
- Caching layer (e.g., Redis) for hot queries  
- Read replicas for database scaling  