# Repository Guide

## Project Overview

Woo is a reusable .NET library collection for service applications. The solution is composed of focused class libraries for CQRS, web integration, persistence, messaging, observability, authentication, resilience, validation, and testing support.

Target framework and package versions are defined in the project files and `Directory.Build.props`; do not duplicate versions in this guide.

## Repository Structure

- `src/Core`: domain contracts, CQRS, events, exceptions, and pagination.
- `src/Web`: ASP.NET Core helpers, API versioning, options, and service registration.
- `src/EFCore`, `src/Mongo`, `src/EventStoreDB`: persistence integrations.
- `src/Wolverine`, `src/Caching`, `src/Validation`, `src/Logging`, `src/Polly`: cross-cutting behaviors and messaging.
- `src/Jwt`, `src/OpenApi`, `src/OpenTelemetryCollector`, `src/HealthCheck`, `src/ProblemDetails`, `src/Mapster`, `src/Utils`: integration modules.
- `src/TestBase`: reusable integration-test fixtures and Testcontainers support.
- `assets`: repository assets.
- `woo.slnx`: solution entry point.

## Build and Validation

Run from repository root:

```bash
dotnet restore woo.slnx
dotnet build woo.slnx --no-restore
```

There are currently no test project files in the solution. Do not add a `dotnet test` CI step until a test project exists; use `src/TestBase` from downstream integration tests.

## Conventions

- Keep NuGet references in the library that directly uses them. Do not recreate a shared dependency class library.
- Preserve focused project boundaries. Add a `ProjectReference` only when source APIs from another Woo library are required.
- Put public registration APIs in the owning module's `Extensions.cs` or equivalent extension class.
- Keep nullable reference types and implicit usings enabled through `Directory.Build.props`.
- Use the existing namespace and file-scoped namespace style.
- Avoid unrelated formatting or package upgrades in feature changes.
- Update README, architecture documentation, and CHANGELOG when public module boundaries or contributor workflows change.

## Adding a Module or Feature

1. Place source under the owning `src/<Module>` folder.
2. Add only direct package references to that module's `.csproj`.
3. Add project references for Woo APIs consumed by the module.
4. Add or update the module's public registration extension.
5. Update `woo.slnx` if adding a new project.
6. Update `README.md`, `docs/how-it-works.md`, and `CHANGELOG.md` when the public surface or architecture changes.
7. Run `dotnet restore woo.slnx` and `dotnet build woo.slnx --no-restore`.

## CI/CD

`.github/workflows/ci.yml` builds the solution on pushes and pull requests. `.github/workflows/copilot-setup-steps.yml` prepares the same .NET build environment for coding agents. `.github/workflows/release-drafter.yml` remains the release-note automation.

## Documentation Status

The main module catalog is in `README.md`. Architectural ownership and dependency flow are documented in `docs/how-it-works.md`. Contributor process details remain in `CONTRIBUTION.md`.

## Common Pitfalls

- A successful restore does not prove package ownership is correct; build the full solution after changing a `.csproj`.
- Do not reintroduce `src/Dependencies/Woo.Dependencies.csproj`; package ownership is intentionally local.
- `src/TestBase` is support infrastructure, not an executable test project.
- Integration modules may require PostgreSQL, MongoDB, EventStoreDB, or RabbitMQ when consumed by an application, but the library build itself does not start those services.
