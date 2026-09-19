# Hoeyer.OpcUa - Agent Instructions

## Project overview
A small OPC UA framework built on OPC Foundation's UA-.NETStandard. It uses reflection and
source generation to provide a code-first workflow: a C# class annotated with
`OpcUaEntityAttribute` defines an OPC UA Node (an "entity"), and client + server artifacts
are generated from it.

Two main parts:
1. Client - services that browse/write/read the defined entities.
2. Server - spins up a basic server, mainly for testing and small simulations.

## Solution layout (root = this folder, `Hoeyer.OpcUa.sln`)
- `Hoeyer.OpcUa.Core{,.Abstractions,.Configuration,.SourceGeneration}` - entity model, DI/options configuration, Roslyn source generator.
- `Hoeyer.OpcUa.Server{,.Abstractions}` - the OPC UA server (`OpcEntityServer`, `StartableEntityServer`).
- `Hoeyer.OpcUa.Client{,.Abstractions}` - client-side services to the defined entities.
- `Hoeyer.OpcUa.Simulation.*` - simulation engine that generates fake OPC UA data.
- `Hoeyer.Common` - shared helpers.
- `Playground.*` - runnable example/console apps.
- Test projects in each `*.Test`, plus `Hoeyer.OpcUa.ClientServer.IntegrationTest`.
- `Hoeyer.UnitTests.slnf` - solution filter with just the unit test projects.

## Commands
- Build: `dotnet build Hoeyer.OpcUa.sln`
- All tests use TUnit. Docs: https://tunit.dev
- Unit tests: `dotnet test Hoeyer.UnitTests.slnf`
- Other runnable apps live under `Playground.*`;


## Testing with TUnit
- `global.json` opts `dotnet test` into the Microsoft.Testing.Platform runner (required on the .NET 10 SDK, where VSTest mode is removed).
- Preferred invocation to pass test flags (e.g. `--report-trx`): run the test project/app directly, so flags are forwarded to the test app:
  `dotnet run --project Hoeyer.OpcUa.ClientServer.IntegrationTest -- --report-trx`
- TUnit writes HTML + TRX reports into `bin\<Config>\<tfm>\TestResults\` in the test project.
- Known quirk: `dotnet test` may report "Zero tests ran" (exit code 5) on the integration test project - fall back to running the built test executable.

## Integration test environments
`Hoeyer.OpcUa.ClientServer.IntegrationTest` (net10.0) abstracts where the OPC UA server under test lives, under `EnvironmentAdapter\`:
- `IIntegrationTestEnvironment` - defines `IServiceProvider AvailableServices`, `OpcEnvironment`, and `Task<bool> EnvironmentReady()`; implements `IAsyncInitializer` + `IAsyncDisposable`.
- `IntegrationTestAdapter` - static registry to plug in an `IIntegrationTestEnvironmentAdapterFactory` (`AssignAdapter`/`AssignFuncFactory`); adapters are cached per key (`CreateOrGetCached`, `GetSessionIsolatedAdapter`).
- `LocalHostedIntegrationTestEnvironment` + `AssignLocallyHostedEnvironment` (`[Before(TestDiscovery)]`) - boots an in-process OPC UA server on a free loopback port; the localhost default.
- `DockerEnvironmentAdapter` + `AssignTestContainerEnvironment` (in `Playground.Application.EndToEndTest`) - containerized alternative backed by Testcontainers; registered with a higher discovery order.
- `IntegrationAdapterDependentTest` - TUnit `SkipAttribute` that skips tests when no environment adapter/factory is registered (`NoFrameworkAdapterException`).

## Conventions
- Libraries target `netstandard2.1`; test/playground apps target `net9.0`/`net10.0` (see each `.csproj`).
- Central package management: versions in root `Directory.Packages.props`, references in the owning `.csproj` (do NOT put versions in `.csproj`).
- Root `Directory.Build.props`: `Nullable` enable, `ImplicitUsings` enable, `LangVersion` latestmajor.
- Namespaces: `Hoeyer.<ProjectArea>.*`. Keep interfaces in `*.Abstractions` projects.
- OPC UA API used: `OPCFoundation.NetStandard.Opc.Ua` v1.5.x.

## OPC UA documentation
Always consult `.opencode/opc-ua-docs.md` for official documentation links and API lookup guidance
before writing OPC UA code. Prefer the official spec and OPC Foundation source over guessing.

## Known bugs
`known-bugs.md` at the repo root is the registry of known defects. Check it before touching related code,
and fix them in severity order (Critical → Medium → Low).

## General agent rules
- Verify with build/tests after making changes (`/build`, `/test`).
- Match existing patterns (source generation, DI registration style) rather than inventing new ones.