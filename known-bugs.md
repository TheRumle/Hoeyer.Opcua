# Known Bugs

Registry of known defects in this repo. Fix in order of severity. After fixing, re-run the
integration tests (see `.opencode/tunit-docs.md`) to verify, then move the entry to
`fixed-bugs.md`. Resolved defects (e.g. BUG-001) are archived there. Evidence below is from
the run on 2026-09-19 (`Hoeyer.OpcUa.ClientServer.IntegrationTest`, 184 tests: 149 ok, 4 failed,
31 skipped).

## Critical

### BUG-002 — Disposing a failed/partially-started server throws on `MasterNodeManager.Dispose`
- Location: `Hoeyer.OpcUa.Server/OpcEntityServer.cs:200` + `StartableEntityServer.cs:51-67`; exception surfaces from OPC Foundation `MasterNodeManager.Dispose`.
- Symptom: `System.ObjectDisposedException: Cannot access a disposed object. Object name: 'System.Threading.SemaphoreSlim'` when tearing down after a failed start.
- Evidence: trace walks `StartableEntityServer.DisposeAsync → OpcEntityServer.Dispose → DomainManager?.Dispose → MasterNodeManager.Dispose → SemaphoreSlim.Wait`.
- Impact: masks the real error (formerly BUG-001), produces noisy dispose exceptions in every failed run.
- Fix direction: only dispose server internals when startup reached a valid state; make the server dispose chain tolerant of partial initialization.

### BUG-003 — Session-open `BadRequestTimeout` at run start + fixture dispose issues
- Location: `Hoeyer.OpcUa.ClientServer.IntegrationTest/Fixtures/IntegrationFixtureResources.cs:15-26` (+ `LocalHostedIntegrationTestEnvironment`).
- Symptom (run 2026-09-19 second pass, e.g. port 56604): the 4 still-failing tests ALL fail in the opening wave (t≈08:46:53) at session creation:
  - browsers: `BadRequestTimeout` `[80850000]` from `UaSCUaBinaryClientChannel.ConnectAsync` (server does not answer Hello/Acknowledge inside the client timeout),
  - `"Can connect to 1 session"`: full 5 s `[Timeout]` elapsed before its async session-open completed.
- Evidence against mid-run server teardown: 149 tests passed *after* the failing wave on the same single server, so the server stays reachable — it just fails to serve the opening burst of parallel session opens promptly. Identity is flaky/port-dependent (run 1: 4 connection failures; run 2: same 4, all `BadRequestTimeout`).
- Related dispose defect (confirmed again, session end): `NullReferenceException` at `IntegrationFixtureResources.DisposeAsync:17` — `ServerEnvironment` is `null` for fixtures disposed without ever initializing (skipped classes still get tracked/disposed). Logged as "Error disposing tracked object at session end".
- Structural design flaw (latent): every class fixture currently wraps the *same* session-isolated environment (`PerTestSessionKey`) and `DisposeAsync` disposes `ServerEnvironment` (the shared server) whenever *any* class finishes. The comment on `IntegrationServiceInjectionAttribute` already states the intent: "The environment itself is session-owned and must not be disposed by this data source."
- Fix directions (decision needed): (a) fixtures should only dispose their own `ServiceProvider` scope and the shared environment should be disposed once at session end; (b) address the opening-wave latency — e.g. pre-warm the server after startup (open a throwaway session) or de-serialize the first session opens (`[NotInParallel]`) so the in-process server isn't hammered before it is warm.

## Medium

### BUG-004 — Alarm wiring is dead code + `EvaluateAlarm` throws
- Location: `Hoeyer.OpcUa.Server/Application/AlarmSetupConfigurator.cs:12-13,50-53`
- Symptom: `return;` is the first statement in the `ChangeState` lambda → the alarm-creation loop never runs (compiler warns CS0162 unreachable code). `EvaluateAlarm` even if reached throws `NotImplementedException`.
- Impact: `AlarmsByProperty` handling silently does nothing; any code path touching it crashes.
- Fix direction: remove the premature return; implement `EvaluateAlarm`.

## Low / code-quality

### BUG-005 — Dead field `EntityNodeManager<T>._nodeTask` (resolved)
- Location: `Hoeyer.OpcUa.Server/Application/EntityNodeManager.cs:22`
- Resolved as part of the BUG-001 fix (`fixed-bugs.md`): `_nodeTask` is now assigned in `CreateAddressSpace`. Verify no new CS0414 (assigned-but-unused) warning surfaces; if it does, delete the field and re-assert `NodeReady` via `_addressSpaceReady.Task`. Ensure the shipped build still shows no warnings.

### BUG-006 — Misc nullable/obsolete warnings (log noise, not yet defects)
- `Hoeyer.OpcUa.Client/Extensions/LoggingExtensions.cs:39,43` — possible null deref on `item.Subscription` (CS8602/CS8604).
- `Hoeyer.OpcUa.Simulation.Application/Services/ServiceCollectionExtension.cs:119,123` — possible null assignment into `args` list (CS8601) around `ExecuteLocalStaticGeneric`.
- `Hoeyer.OpcUa.Client/Application/Connection/DefaultReconnectStrategy.cs:28` — unused `_` exception variable (CS0168).
- Obsolete API usage across `CertificateBasedSecurityConfigurationFactory.cs:60`, `ServerApplication.cs:10` (CS0618) — OPC Foundation moved to `ITelemetryContext`-based constructors; address when upgrading the SDK.

## Auto-generated suspect

### BUG-007 — `dotnet test` reports "Zero tests ran" (exit code 5) on the integration project
- This is tooling, not product code: the MTP runner (see `global.json` + `.opencode/tunit-docs.md`) miscounts discovery under `dotnet test`. Reliable path is running the built test app directly. Kept here so it is not re-diagnosed as a product bug.