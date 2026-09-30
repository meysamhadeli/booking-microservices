# How It Works

Woo is a collection of independently consumable .NET class libraries. `woo.slnx` contains the modules under `src/`; there is no application host in this repository.

## Dependency Direction

`Core` contains shared contracts and domain primitives. `Web` contains ASP.NET Core integration and is consumed by infrastructure modules that need web registration. Persistence and operational adapters keep their provider-specific packages in their own project files. Cross-cutting modules such as `Caching`, `Validation`, `Logging`, `Polly`, and `Wolverine` expose focused registration or pipeline behavior.

Every project declares the NuGet packages used by its own source. A Woo `ProjectReference` is reserved for source APIs from another Woo module; packages are not inherited through a dependency bucket.

## Registration Flow

Consumer applications typically register the relevant module extensions from `src/*/Extensions.cs` during startup. The registration chain is intentionally opt-in: adding a project reference does not automatically enable every infrastructure provider.

- Web concerns begin in `src/Web`.
- EF Core, MongoDB, and EventStoreDB concerns begin in their provider modules.
- Messaging and request pipeline concerns begin in `src/Wolverine`, `src/Caching`, `src/Validation`, and `src/Logging`.
- OpenAPI, health, JWT, problem-details, and telemetry setup live in their named modules.

## Build Boundary

The canonical validation command is:

```bash
dotnet restore woo.slnx
dotnet build woo.slnx --no-restore
```

`src/TestBase` supplies fixtures and Testcontainers dependencies for downstream integration tests. The repository currently has no test project of its own.
