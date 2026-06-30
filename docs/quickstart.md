# Quickstart

This guide gets the template running locally with a reliable baseline for development.

## Prerequisites

- .NET SDK 10
- Git
- Bruno (optional, for API request testing)
- Entra ID account (optional, for authentication scenarios)

If you use GitHub Copilot agent workflows, install `dotnet/skills` and the custom skills used by this repository before starting.

## 1. Clone the Repository

```bash
git clone <repository-url>
cd Template.WebAPI.CleanArchitecture
```

## 2. Restore, Build, and Test

```bash
dotnet restore ProjectName.slnx
dotnet build ProjectName.slnx
dotnet test ProjectName.slnx
```

## 3. Run the Application

Run API only:

```bash
dotnet run --project src/ProjectName.Web.Api/ProjectName.Web.Api.csproj
```

Run with AppHost orchestration:

```bash
dotnet run --project src/ProjectName.AppHost/ProjectName.AppHost.csproj
```

## 4. Verify Local Endpoints

- OpenAPI JSON artifact: `api/ProjectName.Web.Api.json`
- OpenAPI endpoint (development): `/openapi/v1.json` (configuration dependent)
- Scalar UI (development): `/scalar/v1`
- Health endpoint (development): `/health`

## 5. Validate API Contracts with Bruno (Optional)

1. Configure environment values using [bruno-env-setup.md](bruno-env-setup.md).
2. Open the `bruno` collection.
3. Execute requests for your target feature slice.

## Troubleshooting

### Build warnings or analyzer failures

- Treat warnings as actionable quality signals.
- Resolve warnings in touched projects before merging changes.
- Use `.editorconfig` as the source of truth for style and analyzer behavior.

### Authentication scenarios fail locally

- Validate Entra ID configuration values in `appsettings` files.
- Confirm token audience/scope values match your local environment.

### Persistence or local database issues

- Verify persistence configuration and connection settings.
- Confirm expected database artifacts are available in the `database` folder or configured provider.
