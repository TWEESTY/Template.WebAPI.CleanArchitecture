# Chapter 4: Tests and Quality

## Objective

Establish a reliable quality baseline that protects architecture and contract integrity as the template evolves.

## Test Scope by Layer

### Application tests

- Validate command/query handler behavior.
- Validate expected failures and business rule outcomes.
- Validate FluentValidation rules (invalid inputs return `ValidationError`).
- Verify state changes for mutating use cases.

### Web API tests

- Validate route contracts and status codes.
- Validate serialization and payload contracts.
- Validate API-level error mapping behavior.
- Verify `ValidationError` maps to `ValidationProblem` (HTTP 400) for relevant endpoints.

## Commands

Run all tests:

```bash
dotnet test ProjectName.slnx
```

Run by test project:

```bash
dotnet test tests/ProjectName.Application.Tests/ProjectName.Application.Tests.csproj
dotnet test tests/ProjectName.Web.Api.Tests/ProjectName.Web.Api.Tests.csproj
```

## Test Authoring Standards

- Use naming pattern: `Method_WhenCondition_ShouldExpectedResult`.
- Keep each test focused on one observable behavior.
- Prefer explicit assertions over generic assertions.
- Avoid hidden logic inside tests.
- Keep test arrangement readable and deterministic.

## FluentValidation Test Guidance

- For each new command/query validator, add at least one failing-input test.
- Assert the result is failed and contains `ValidationError` with the expected identifier.
- For mutating commands, assert no invalid data was persisted when validation fails.

## Static Quality Expectations

- Use `.editorconfig` as the canonical style/analyzer source.
- Fix warnings in touched scope before merging.
- Keep suppressions rare and justified.

## Recommended CI Quality Gate

1. Restore
2. Build
3. Test
4. Style/analyzer validation
5. Optional contract checks (OpenAPI drift and Bruno alignment)
