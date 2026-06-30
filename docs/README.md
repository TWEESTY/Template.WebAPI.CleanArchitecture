# Documentation Hub

This documentation is organized for onboarding, day-to-day development, and production readiness.

## Authentication Model

The template supports two authentication entry patterns:

- BFF web app flow using authenticated session cookies for browser-to-API calls.
- Bearer access token flow for API-to-API and service-to-service calls.

For endpoint-level examples, see [Use Cases](use-cases.md).

## Start Here

- [Quickstart](quickstart.md)
- [Use Cases](use-cases.md)
- [Bruno Environment Setup](bruno-env-setup.md)

If you use GitHub Copilot agent workflows, install `dotnet/skills` and the repository custom skills before following the quickstart.

## Architecture and Development

1. [Clean Architecture and CQRS](chapters/01-clean-architecture-cqrs.md)
2. [Create an Endpoint](chapters/02-create-an-endpoint.md)
3. [REST API Best Practices](chapters/03-best-practices-rest-api.md)

Validation note: this template uses FluentValidation in the Application mediator pipeline, with endpoint mapping from `ValidationError` to `ValidationProblem`.

## Quality and Operations

4. [Tests and Quality](chapters/04-tests-and-quality.md)
5. [Observability, OpenAPI, and Health Checks](chapters/05-observability-openapi-healthchecks.md)

## Recommended Learning Paths

### Path A: First-time user

1. [Quickstart](quickstart.md)
2. [Use Cases](use-cases.md)
3. [Clean Architecture and CQRS](chapters/01-clean-architecture-cqrs.md)
4. [Create an Endpoint](chapters/02-create-an-endpoint.md)

### Path B: API maintainer

1. [REST API Best Practices](chapters/03-best-practices-rest-api.md)
2. [Tests and Quality](chapters/04-tests-and-quality.md)
3. [Bruno Environment Setup](bruno-env-setup.md)

### Path C: Platform/DevOps

1. [Observability, OpenAPI, and Health Checks](chapters/05-observability-openapi-healthchecks.md)
2. [Tests and Quality](chapters/04-tests-and-quality.md)

## Copilot Agent Guidance

- `Dotnet Use Case`: use for feature slice updates (use case + endpoint + contract alignment).
- `Dotnet Tests`: use for application and API test coverage updates.