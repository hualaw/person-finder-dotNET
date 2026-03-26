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

## 7. Trade-offs

* PostGIS introduces complexity but provides significant performance gains
* Clean architecture adds initial overhead but improves long-term maintainability

---

## 8. Failure Handling

* Handle invalid coordinates
* Gracefully manage database connectivity issues
* Support retry mechanisms for transient failures

---
