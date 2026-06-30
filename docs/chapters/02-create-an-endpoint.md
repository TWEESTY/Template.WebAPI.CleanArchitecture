# Chapter 2: Create an Endpoint

## Objective

Add a new endpoint that is consistent with the repository's vertical-slice CQRS conventions.

## Step 1: Add the Use Case in Application

Create command/query files in the feature slice:

- `src/ProjectName.Application/<Feature>/Commands`
- `src/ProjectName.Application/<Feature>/Queries`

Implementation rules:

- Commands implement `ICommand<Result<TResponse>>`.
- Queries implement `IQuery<Result<TResponse>>`.
- Handlers return `ValueTask<Result<TResponse>>`.
- Use repository abstractions from Application `Common`.
- Add a FluentValidation validator (`AbstractValidator<TCommandOrQuery>`) in the same slice file/folder as the use case.
- Validation runs in the Application mediator pipeline and returns `ValidationError` entries in `Result`.

Query return strategy:

- Simple CRUD reads can use Domain entities.
- Non-CRUD reads should return projected Application DTOs directly from repository methods.

## Step 2: Add Endpoint File in Web API

Create one endpoint file per use case:

- `src/ProjectName.Web.Api/<Feature>/Commands/<UseCase>Endpoint.cs`
- `src/ProjectName.Web.Api/<Feature>/Queries/<UseCase>Endpoint.cs`

Endpoint responsibilities:

- Bind request data.
- Dispatch command/query via `IMediator`.
- Map `Result<TApplicationDto>` to typed HTTP results.
- Keep presentation contracts separate from Application DTOs.

## Step 3: Register in Feature Endpoint Group

Update `<Feature>EndpointsGroup.cs`:

- Add route mapping with `MapGet`, `MapPost`, `MapPut`, or `MapDelete`.
- Keep routing and tags aligned with existing feature conventions.

## Standard Error Mapping

- `UnauthorizedError` -> `TypedResults.Unauthorized()`
- `ForbiddenError` -> `TypedResults.Forbid()`
- `NotFoundError` -> `TypedResults.NotFound()`
- `ValidationError` -> `TypedResults.ValidationProblem(...)`
- fallback -> `TypedResults.InternalServerError()`

## Validation Checklist

- Prefer FluentValidation for input validation in Application use cases.
- Keep business invariants in Domain entities/behavior methods.
- Do not validate in endpoint handlers beyond request binding concerns.
- Ensure endpoint maps `ValidationError` to `ValidationProblem`.

## Definition of Done

- Endpoint appears in OpenAPI.
- Endpoint contains no business rules.
- Domain/Application handle business validation.
- Use case has a FluentValidation validator when request input constraints exist.
- Tests are added or updated.
- Bruno request files are added or updated when route, payload, auth, query, or response contracts change.
