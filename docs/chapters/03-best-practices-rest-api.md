# Chapter 3: REST API Best Practices

## Objective

Maintain a consistent, predictable API surface that is easy to consume and safe to evolve.

## Resource and Route Design

- Use plural, resource-oriented routes (for example `/api/owners`, `/api/pets`).
- Let HTTP methods describe actions.
- Keep route patterns consistent across feature slices.
- Avoid command verbs in route names when resource semantics are sufficient.

## Response Codes

- `200 OK`: successful read or update with payload.
- `201 Created`: successful create with resource location/payload.
- `204 No Content`: successful operation with no payload (common for deletes).
- `400/422`: validation or request contract failures.
- `401 Unauthorized`: request is unauthenticated.
- `403 Forbidden`: request is authenticated but not authorized.
- `404 Not Found`: resource not found.
- `500 Internal Server Error`: unexpected error path.

## Contract Boundaries

- Do not expose Domain entities directly over HTTP.
- Keep Application DTOs use-case oriented.
- Keep Web API presentation contracts independent from Application DTOs.
- Map Application DTOs to API response contracts in endpoint handlers.
- Endpoint versioning is intentionally not implemented in this template for clarity; introduce an explicit versioning strategy before breaking contract changes in production APIs.

## Validation and Error Consistency

- Transport validation belongs to endpoint/request binding concerns.
- Use-case validation belongs to Application.
- Invariants belong to Domain.
- Return consistent validation responses using `ValidationProblem` shape.

## Security Defaults

- Prefer secure-by-default endpoint groups.
- Keep unauthorized/forbidden responses explicit.
- Avoid leaking implementation details in production errors.
- Never log secrets, tokens, or private credentials.

## Operational Consistency

- Keep OpenAPI descriptions synchronized with endpoint contracts.
- Maintain health checks for critical dependencies.
- Ensure logging and telemetry support request-level diagnostics.
