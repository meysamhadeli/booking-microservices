# Copilot Instructions

## Scope

This repository is a .NET 10 library solution. Read the nearest module project file before changing source so package ownership and Woo project dependencies stay local.

## Code and Project Conventions

- Use file-scoped namespaces, nullable reference types, and implicit usings as configured in `Directory.Build.props`.
- Preserve public extension-method registration patterns in each module.
- Add a NuGet package to the consuming `.csproj`; do not add a new shared package bucket.
- Keep `ProjectReference` edges limited to source contracts or integration APIs actually consumed.
- Prefer existing abstractions in `src/Core` over new cross-module contracts.
- Keep changes narrowly scoped and avoid opportunistic package upgrades.

## Validation

```bash
dotnet restore woo.slnx
dotnet build woo.slnx --no-restore
```

No test project is currently present. `src/TestBase` contains reusable fixtures for downstream integration tests; do not claim local test coverage unless a test project is added.

## Module Patterns

- `src/Core` owns events, CQRS contracts, exceptions, and shared models.
- `src/Web` owns ASP.NET Core registration and web-facing helpers.
- Persistence adapters own their provider packages and registration extensions.
- `src/Wolverine`, `src/Caching`, `src/Validation`, and `src/Logging` expose cross-cutting pipeline behavior.
- `src/OpenApi`, `src/OpenTelemetryCollector`, and `src/HealthCheck` own observability and operational registrations.

## Maintenance Matrix

| Change | Also inspect or update |
|---|---|
| Add/remove Woo module | `woo.slnx`, module `.csproj`, `README.md`, `docs/how-it-works.md`, `CHANGELOG.md` |
| Add a package or provider API | Consuming module `.csproj`, `Directory.Build.props` if build-wide, full solution build |
| Change a public event, CQRS contract, exception, or model | `src/Core`, consuming adapters, `README.md`, `docs/how-it-works.md` |
| Change ASP.NET registration | `src/Web`, affected module `Extensions.cs`, `README.md` |
| Change persistence behavior | Provider module (`src/EFCore`, `src/Mongo`, or `src/EventStoreDB`), registration callers, docs |
| Change messaging behavior | `src/Wolverine`, `src/Core/Event`, downstream registration and integration fixtures |
| Change cache/validation/logging pipeline | Relevant behavior module, MediatR registration in `src/Web`, docs |
| Change OpenAPI, health, or telemetry setup | Owning module `Extensions.cs`, consuming app registration, CI build |
| Change test fixtures | `src/TestBase`, provider packages, downstream test setup, `CHANGELOG.md` |
| Change contributor or build workflow | `CONTRIBUTION.md`, `README.md`, `.github/workflows/`, `AGENTS.md` |

## Review Expectations

Check direct package ownership, project-reference direction, public API compatibility, nullability warnings introduced by the change, and full-solution build output. Do not suppress warnings or add broad package references to make a single project compile.

## Documentation

Keep `README.md` as the contributor-facing module catalog, `docs/how-it-works.md` as the architecture explanation, and `AGENTS.md` as the repository operating guide. Update `CHANGELOG.md` for meaningful public or workflow changes.
