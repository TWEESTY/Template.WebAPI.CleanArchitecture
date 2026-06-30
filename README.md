# Template.WebAPI.CleanArchitecture

A production-oriented .NET Web API template for teams that want Clean Architecture, vertical-slice CQRS, and strong engineering conventions from day one.

This repository is designed to be shared with the .NET community as a practical, opinionated reference for building maintainable APIs.

## Application Use Cases

The sample application models a veterinary clinic platform. It demonstrates authenticated account flows, clinic and veterinarian management, owner and pet records, appointment scheduling, vaccine catalog maintenance, pet vaccine administration history, ownership transfer, and external dog breed lookup.

These business flows are intentionally broad enough to show simple CRUD, nested aggregate operations, workflow state changes, external API integration, validation, authorization, and contract-tested Minimal API endpoints. For the full use-case catalog, see [docs/use-cases.md](docs/use-cases.md).

## Authentication Model

This template implements a BFF authentication flow for browser clients and supports token-based calls for service clients:

- Web app to Web API: the web app authenticates through the BFF flow and calls this API using the authenticated user session (cookie-based auth).
- Web API to Web API: non-browser clients can call this API using OAuth/OIDC access tokens (bearer token auth).

This dual mode keeps browser authentication secure and user-centric while still enabling machine-to-machine integrations.

## Why This Template

- Clean Architecture boundaries with explicit dependency rules.
- Vertical-slice CQRS organization by feature.
- Minimal APIs with typed results and predictable error mapping.
- FluentValidation integrated in Application mediator pipeline.
- Test-first workflow across Application and API layers.
- OpenAPI, health checks, and observability-ready setup.
- Bruno collection aligned with endpoint contracts.

## Quick Start

### Prerequisites

- .NET SDK 10
- Git
- Bruno (optional, for API testing)
- Entra ID account (optional, for auth flows)
- If you use GitHub Copilot agent workflows: install `dotnet/skills` and the custom skills used by this repository.

### Run Locally

```bash
dotnet restore ProjectName.slnx
dotnet build ProjectName.slnx
dotnet test ProjectName.slnx
```

Run API only:

```bash
dotnet run --project src/ProjectName.Web.Api/ProjectName.Web.Api.csproj
```

Run full orchestration with AppHost:

```bash
dotnet run --project src/ProjectName.AppHost/ProjectName.AppHost.csproj
```

### Verify

- OpenAPI JSON: `api/ProjectName.Web.Api.json`
- Scalar UI (development): `/scalar/v1`
- Health endpoint (development): `/health`

For a step-by-step setup guide, see [docs/quickstart.md](docs/quickstart.md).

## Documentation

Start at the documentation hub: [docs/README.md](docs/README.md)

### Recommended Paths

- New to the template:
	- [docs/quickstart.md](docs/quickstart.md)
	- [docs/use-cases.md](docs/use-cases.md)
	- [docs/chapters/01-clean-architecture-cqrs.md](docs/chapters/01-clean-architecture-cqrs.md)
	- [docs/chapters/02-create-an-endpoint.md](docs/chapters/02-create-an-endpoint.md)
- API quality and consistency:
	- [docs/chapters/03-best-practices-rest-api.md](docs/chapters/03-best-practices-rest-api.md)
	- [docs/chapters/04-tests-and-quality.md](docs/chapters/04-tests-and-quality.md)
- Operability and tooling:
	- [docs/chapters/05-observability-openapi-healthchecks.md](docs/chapters/05-observability-openapi-healthchecks.md)
	- [docs/bruno-env-setup.md](docs/bruno-env-setup.md)

## Repository Layout

```text
src/
	ProjectName.Web.Api/          # HTTP layer (Minimal APIs)
	ProjectName.Application/      # CQRS use cases and contracts
	ProjectName.Domain/           # Domain model and invariants
	ProjectName.Infrastructure/   # Persistence and external integrations
	ProjectName.AppHost/          # Aspire orchestration host
tests/
	ProjectName.Application.Tests/
	ProjectName.Web.Api.Tests/
docs/
	README.md                     # Documentation hub
	quickstart.md
	chapters/
bruno/                          # API contract examples and manual tests
api/                            # Generated OpenAPI artifacts
```

## Community and Contributions

Issues and pull requests are welcome. When contributing:

- Preserve layer boundaries and vertical-slice conventions.
- Keep endpoint contracts and Bruno files synchronized.
- Add or update tests for behavioral changes.
- Keep documentation current when architecture or workflow changes.

## Copilot Agents

- `Dotnet Use Case`: create or modify use cases and endpoints while preserving architecture and contract conventions.
- `Dotnet Tests`: add or update tests aligned with repository standards.