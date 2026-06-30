# Chapter 1: Clean Architecture and CQRS

## Objective

Understand how code is organized, how requests flow through layers, and how to keep boundaries stable as the solution grows.

## Layer Responsibilities

### Domain

- Owns business concepts, invariants, and behavior.
- Contains no infrastructure, framework, or HTTP concerns.

### Application

- Implements use cases through CQRS commands and queries.
- Defines contracts and abstractions consumed by Infrastructure.
- Coordinates domain behavior and expected error outcomes.

### Infrastructure

- Implements application abstractions (persistence, external integrations, auth adapters).
- Contains EF Core and external system wiring.

### Web API

- Hosts Minimal API endpoints.
- Maps HTTP contracts to Application use cases and responses.
- Performs transport-level responsibilities only.

## Dependency Rules

- Web API -> Application
- Infrastructure -> Application
- Application -> Domain
- Domain -> no dependency on outer layers

Any dependency that violates this direction weakens maintainability and testability.

## CQRS Model in This Template

- Commands change state.
- Queries read state.
- Endpoints dispatch commands/queries through `IMediator`.

Repository return strategy:

- Simple CRUD queries may return Domain entities.
- Non-CRUD read scenarios (search/list/filter/sort) should project directly to Application DTOs.

## Request Flow

1. Endpoint receives HTTP input.
2. Endpoint creates command/query.
3. Handler executes use case.
4. Repository or service in Infrastructure performs technical work.
5. Handler returns `Result<T>`.
6. Endpoint maps result to typed HTTP response.

## Guardrails

- Keep business logic out of endpoints.
- Keep Infrastructure out of Application and Domain.
- Enforce domain invariants in Domain.
- Represent expected failures with `Result<T>` and application errors.
- Keep Application DTOs separate from Web API presentation contracts.
