# Repository Copilot Instructions

This repository follows strict Clean Architecture and consistent vertical-slice conventions.
All generated code must align with the existing patterns below.

## Copilot and Agent Routing

- For new or modified use cases, CQRS handlers, Minimal API endpoints, REST routes, feature slices, OpenAPI contracts, or Bruno requests, prefer the `Dotnet Use Case` agent.
- For adding or updating Application handler tests, Web API contract tests, endpoint regression coverage, or Bruno-related test coverage, prefer the `Dotnet Tests` agent.
- For broad read-only discovery, first inspect nearby feature slices and existing tests before editing; keep implementation agents focused on the smallest relevant slice.
- The workspace RTK hook already nudges terminal commands through `rtk`; still explicitly write validation commands with the `rtk` prefix in plans and summaries.

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
- For command/query input validation, use FluentValidation validators (`AbstractValidator<TRequest>`) in the Application slice.
- Prefer validation through the mediator pipeline behavior, returning `ValidationError` in `Result` for invalid inputs.
- When create/update commands share the same validation rules, mutualize them in a feature-scoped validator base and a dedicated contract interface named `ICreateOrUpdate<Feature>Command`.
- For expected failures, return `Result.Fail(new <AppError>(...))` instead of throwing technical exceptions.
- For simple CRUD queries (for example by id), repositories may return Domain entities.
- For non-CRUD/read-model queries (for example list/search/filter/sort), prefer repository projection directly to Application DTOs (for example `Get<Feature>Response`) to avoid passing Domain entities through query handlers.
- Keep Application DTOs distinct from Web API presentation DTOs; endpoints map Application DTOs to `*CommonResponseEndpoint` contracts.
- For current time access in Application, Infrastructure, and Web layers, use `TimeProvider` via DI instead of `DateTime.Now`/`DateTime.UtcNow`.
- Prefer passing time from Application services/handlers into Domain behavior rather than coupling Domain entities to infrastructure concerns.

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
- Use suffix form for identifiers: `ClinicId`, `OwnerId`, `VeterinarianId` (never `idClinic`/`IdClinic`).
- Build names from broad context to specific meaning, with technical role at the end.
- For class names, use domain/feature first and role suffix last (for example `CreateOwnerCommand`, `GetClinicByIdQuery`, `UpdateVeterinarianCommandHandler`, `ClinicRepository`, `GetOwnerByIdEndpoint`).
- Keep role suffixes explicit and consistent when applicable: `Command`, `Query`, `Handler`, `Endpoint`, `Repository`, `Request`, `Response`.
- Avoid vague class names such as `Manager`, `Helper`, or `Utils` unless they are truly the most precise name.
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

### Naming Examples (Good vs Bad)

- Good: `ClinicId`, `OwnerId`, `GetClinicByIdQuery`, `UpdateVeterinarianCommandHandler`.
- Bad: `idClinic`, `IdOwner`, `QueryGetClinicById`, `VeterinarianHandlerUpdateCommand`.
- Rule reminder: domain/context first, technical role last, and `Id` as a suffix.

### Test Naming Convention

- Follow .NET unit testing best practices for test naming.
- Prefer method names in this format: `MethodName_WhenCondition_ShouldExpectedResult`.
- Keep test names descriptive of behavior and expected outcome, not implementation details.
- For Application CQRS tests, use one test class per command/query type.
- Mirror Application folder slices in tests: `tests/ProjectName.Application.Tests/<Feature>/Commands` and `tests/ProjectName.Application.Tests/<Feature>/Queries`.
- Name test files after the CQRS class under test with `Tests` suffix (for example `CreateOwnerCommandTests.cs`, `GetOwnerByIdQueryTests.cs`).
- For mutating command tests, always verify the persisted database state with `AppDbContext` after sending the command.
- For time-dependent tests, use `Microsoft.Extensions.TimeProvider.Testing` (`FakeTimeProvider`) instead of relying on real system time.
- When adding or changing FluentValidation rules, add/update tests that assert the failed `Result` contains `ValidationError` and, for mutating commands, that invalid data is not persisted.

### Test Infrastructure

- Application handler tests live in `tests/ProjectName.Application.Tests/` and use `SqliteApplicationTestFixture` as the base fixture (SQLite in-memory database, full mediator pipeline).
- Use `ApplicationTestDataBuilder` to build and seed domain objects required by the test scenario; never construct raw entity state manually.
- HTTP contract / integration tests live in `tests/ProjectName.Web.Api.Tests/` and extend `CustomWebApiFactory` to start the full pipeline.
- Use `TestAuthHandler` to simulate authenticated requests in Web API tests.
- Use `TestUnitOfWorkManager` when a Web API test needs to override the commit behavior.

## Formatting Rules

- Enable and keep auto-format on save for this workspace.
- If an `.editorconfig` file exists, formatting and style must follow it strictly.
- Copilot must always finish code changes in a formatted state.
- After any Copilot code modification, run formatting for changed files before ending the task.
- Respect existing whitespace/line endings and avoid unrelated reformatting outside touched files.
- Do not use the word "asynchronously" in code comments; it is redundant when the method name already ends with `Async`.

## Build Quality Gate

- Validate the touched slice with the narrowest relevant build or test command before finishing.
- A completed change must not leave build warnings in the touched scope.
- If validation surfaces warnings for the files or projects you changed, fix those warnings before ending the task.

## Endpoint Contract Synchronization

- When creating a new use case exposed by the API, also add or update the corresponding Bruno request files under `bruno/`.
- When modifying an existing endpoint route, payload, authentication requirement, query string, or response contract, update the matching Bruno files in the same change.
- After any endpoint change (new route, changed payload, changed response), regenerate `api/ProjectName.Web.Api.json` by building `src/ProjectName.Web.Api/ProjectName.Web.Api.csproj`.
- Endpoint work is not complete until code, tests, Bruno examples, and `api/ProjectName.Web.Api.json` are all aligned.

## What Copilot Must Avoid

- Do not bypass Application by injecting DbContext directly into endpoints.
- Do not add business logic in Web.Api endpoints.
- Do not leak Infrastructure concerns into Domain/Application.
- Do not introduce alternate architectural patterns that conflict with current CQRS + vertical-slice structure.
- Do not skip guard clauses when adding/changing domain behavior.
- Do not call `SaveChanges` or `SaveChangesAsync` in repositories or command handlers; the pipeline behavior owns it.
- Do not finish endpoint contract work without regenerating `api/ProjectName.Web.Api.json`.

## RTK - Token-Optimized CLI

`rtk` is a CLI proxy that filters and compresses command outputs, saving 60-90% tokens.

Always prefix shell commands with `rtk`:

```bash
# Instead of:              Use:
git status                 rtk git status
git log -10                rtk git log -10
dotnet build               rtk dotnet build
dotnet test                rtk dotnet test
dotnet format              rtk dotnet format
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
