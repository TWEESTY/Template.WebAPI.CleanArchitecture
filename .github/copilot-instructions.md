# Repository Copilot Instructions

This repository follows strict Clean Architecture and consistent vertical-slice conventions.
All generated code must align with the existing patterns below.

## Core Principles (Strict)

- Respect layer boundaries at all times:
- `ProjectName.Web.Api` depends on `ProjectName.Application` only for use cases/contracts.
- `ProjectName.Application` depends on `ProjectName.Domain` and application abstractions.
- `ProjectName.Infrastructure` implements application abstractions and depends on EF/external services.
- `ProjectName.Domain` must stay pure and must not depend on Application, Infrastructure, Web, EF, Mediator, or FluentResults.
- Keep business logic in Domain/Application, never in endpoints.
- Prefer extending existing feature folders over creating cross-cutting ad-hoc folders.

## API Endpoint Pattern (Web.Api)

- Organize by feature aggregate under `src/ProjectName.Web.Api/<Feature>/`.
- Keep one endpoint per use case in dedicated files:
- Commands: `.../Commands/<UseCase>Endpoint.cs`
- Queries: `.../Queries/<UseCase>Endpoint.cs`
- Keep a `<Feature>EndpointsGroup.cs` file responsible for route mapping.
- Endpoint classes are `public static class <UseCase>Endpoint` with `HandleAsync(...)`.
- Endpoint handlers only:
- bind request data,
- send a command/query via `IMediator`,
- map `Result<T>` to typed HTTP results.
- Endpoint handlers must not contain domain/business rules.
- Use typed Minimal API results (`Results<...>`) and `TypedResults.*`.
- Return `ValidationProblem` from `ValidationError` using `ToProblemErrors()`.
- Map common application errors consistently:
- `UnauthorizedError` -> `TypedResults.Unauthorized()`
- `ForbiddenError` -> `TypedResults.Forbid()`
- `NotFoundError` -> `TypedResults.NotFound()`
- `ValidationError` -> `TypedResults.ValidationProblem(...)`
- fallback -> `TypedResults.InternalServerError()`
- Keep endpoint request DTOs as nested records (for example `CreatePetEndpointRequest`).

## Application Layer Pattern (CQRS)

- Organize by feature under `src/ProjectName.Application/<Feature>/` with:
- `Commands/`
- `Queries/`
- `Common/`
- Use CQRS with Mediator interfaces:
- Commands implement `ICommand<Result<TResponse>>`
- Queries implement `IQuery<Result<TResponse>>`
- Handlers are sealed classes implementing `ICommandHandler<,>` or `IQueryHandler<,>`.
- Keep handler signatures consistent with existing code:
- explicit interface `Handle(...)`
- return `ValueTask<Result<T>>`
- use `CancellationToken`
- Use feature repository abstractions from Application `Common` (for example `IClinicRepository`).
- Do not reference Infrastructure from Application.
- For expected failures, return `Result.Fail(new <AppError>(...))` instead of throwing technical exceptions.
- Map domain entities to `Get<Feature>Response` DTOs in the handler.

## Domain Layer Pattern

- Domain entities encapsulate invariants and behavior.
- Use private setters and behavior methods (`Rename`, `ChangeAddress`, etc.) instead of public mutable state.
- Use guard clauses from `ProjectName.Domain.Common.Guards.Guard` for domain validation.
- Prefer:
- `Guard.ThrowIfEmptyOrNull(...)`
- `Guard.ThrowIf(...)`
- `Guard.ThrowIfNot(...)`
- `Guard.ThrowIfNull(...)`
- Domain validation failures throw `DomainException` (via Guard), not FluentResults errors.
- Normalize inputs before assigning entity state (trim/name normalization pattern).
- Keep lifecycle fields (`CreatedAt`, `UpdatedAt`) maintained in entity behavior.

## Infrastructure Pattern

- Implement repositories in `src/ProjectName.Infrastructure/Persistence/Repositories/`.
- Repository implementations must satisfy Application interfaces.
- Use `AsNoTracking()` for read-only queries.
- Keep search/sort/filter behavior aligned with `SearchQueryHelper` + field selector dictionaries.
- Persist through EF Core in repository methods (`AddAsync`, `UpdateAsync`, `DeleteAsync`, etc.).

## Naming and Consistency Rules

- Keep naming consistent with current repository conventions:
- `CreateXCommand`, `UpdateXCommand`, `GetXByIdQuery`, `GetXsQuery`
- `CreateXEndpoint`, `GetXByIdEndpoint`, `<Feature>EndpointsGroup`
- `GetXResponse` (Application) and `GetXCommonResponseEndpoint` (Web.Api mapping)
- Place new files in the same folder structure as existing feature slices.
- Match existing coding style and formatting already present in each file.
- Follow Microsoft .NET/C# naming recommendations strictly for all new/updated code.
- Apply these naming rules consistently:
- Types, methods, properties, enums, and constants: PascalCase.
- Method parameters and local variables: camelCase.
- Private fields: _camelCase.
- Interfaces: prefix with `I` (for example `IClinicRepository`).
- Async methods: suffix with `Async`.
- Use meaningful, intention-revealing names; avoid abbreviations unless they are standard and well-known.

## Formatting Rules

- Enable and keep auto-format on save for this workspace.
- If an `.editorconfig` file exists, formatting and style must follow it strictly.
- Copilot must always finish code changes in a formatted state.
- After any Copilot code modification, run formatting for changed files before ending the task.
- Respect existing whitespace/line endings and avoid unrelated reformatting outside touched files.

## What Copilot Must Avoid

- Do not bypass Application by injecting DbContext directly into endpoints.
- Do not add business logic in Web.Api endpoints.
- Do not leak Infrastructure concerns into Domain/Application.
- Do not introduce alternate architectural patterns that conflict with current CQRS + vertical-slice structure.
- Do not skip guard clauses when adding/changing domain behavior.

## RTK - Token-Optimized CLI

`rtk` is a CLI proxy that filters and compresses command outputs, saving 60-90% tokens.

Always prefix shell commands with `rtk`:

```bash
# Instead of:              Use:
git status                 rtk git status
git log -10                rtk git log -10
cargo test                 rtk cargo test
docker ps                  rtk docker ps
kubectl get pods           rtk kubectl pods
```

Meta commands:

```bash
rtk gain              # Token savings dashboard
rtk gain --history    # Per-command savings history
rtk discover          # Find missed rtk opportunities
rtk proxy <cmd>       # Run raw (no filtering) but track usage
```
