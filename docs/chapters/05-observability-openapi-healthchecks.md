# Chapter 5: Observability, OpenAPI, and Health Checks

## Objective

Keep the service diagnosable, discoverable, and deployment-ready with consistent operational signals.

## OpenAPI

The template provides an OpenAPI contract for the API surface.

- Generated artifact: `api/ProjectName.Web.Api.json`
- Development endpoint: `/openapi/v1.json` (configuration dependent)
- Interactive docs UI (development): `/scalar/v1`

Guidance:

- Treat OpenAPI as part of the contract.
- Update documentation artifacts when endpoint contracts change.
- Keep example payloads aligned with Bruno requests.

## Logging

HTTP logging is enabled for request diagnostics.

Recommended logging shape:

- Method
- Route/path
- Status code
- Duration
- Correlation identifier

Avoid logging sensitive data such as tokens, credentials, and private payload fields.

## Health Checks

The template exposes `/health` in development and includes dependency checks such as EF Core availability.

Recommendations:

- Add health checks for each critical dependency.
- Distinguish liveness from readiness in orchestrated environments.
- Ensure health endpoints support your deployment platform probes.

## OpenTelemetry Readiness

The template includes OpenTelemetry packages to support traces, metrics, and logs.

Operational guidance:

- Define what telemetry is exported and where.
- Filter or redact sensitive fields before export.
- Version observability configuration with application changes.
