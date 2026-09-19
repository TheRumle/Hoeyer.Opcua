# TUnit & Test Execution Reference

## Documentation
- Official docs: https://tunit.dev
- Running your tests: https://tunit.dev/docs/getting-started/running-your-tests
- GitHub: https://github.com/thomhurst/TUnit
- Version used in this repo: pin in root `Directory.Packages.props` (currently 1.61.x).

## Key facts
- **Every test project in this repo uses TUnit.** Discovery is source-generated at compile time; tests run in parallel by default.
- Built on Microsoft.Testing.Platform (MTP). TRX + coverage extensions ship inside the `TUnit` package (`--report-trx`, `--coverage`).
- On the .NET 10 SDK, `dotnet test` must run in MTP mode (VSTest mode was removed). The repo opts in via `global.json`:
  `{ "test": { "runner": "Microsoft.Testing.Platform" } }`

## How to run
- **Preferred** (flags are passed straight to the test app):
  `dotnet run --project <test-project> -- --report-trx`
- `dotnet test <project|slnf>` also works, but CLI flags must come after a `--`.
- Known quirk in this repo: `dotnet test` on the integration project can print "Zero tests ran" with exit code 5. Fall back to the built app:
  `dotnet exec <project>\bin\<Config>\<tfm>\<AssemblyName>.dll --report-trx`

## Where reports are written
TUnit writes artifacts under the test project's output folder:
`<project>\bin\<Config>\<tfm>\TestResults\`
- `<AssemblyName>-windows-<tfm>-report.html`
- `<AssemblyName>_<tfm>_<RID>.trx`

Example from the integration tests:
`Hoeyer.OpcUa.ClientServer.IntegrationTest\bin\Debug\net10.0\TestResults\`