# Known Bugs

Registry of known defects in this repo. Fix in order of severity. After fixing, re-run the
integration tests (see `.opencode/tunit-docs.md`) to verify, then move the entry to
`fixed-bugs.md`. Resolved defects (e.g. BUG-001, BUG-002) are archived there. Evidence below is from
the run on 2026-09-26 (`Hoeyer.OpcUa.ClientServer.IntegrationTest`, 182 tests: 147 ok, 4 failed,
31 skipped); the 4 failures are BUG-003 and are unchanged from the 2026-09-19 run (184 tests: 149 ok,
4 failed, 31 skipped).

## Critical

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
- Status: still open — the 2026-09-26 build still emits
  `AlarmSetupConfigurator.cs(13,13): warning CS0162 Unreachable code detected`.
- Fix direction: remove the premature return; implement `EvaluateAlarm`.

### BUG-006 — Misc nullable/obsolete warnings (log noise)
- `Hoeyer.OpcUa.Client/Extensions/LoggingExtensions.cs:39,43` — possible null deref on `item.Subscription` (CS8602/CS8604).
- `Hoeyer.OpcUa.Simulation.Application/Services/ServiceCollectionExtension.cs:119,123` — possible null assignment into `args` list (CS8601) around `ExecuteLocalStaticGeneric`.
- `Hoeyer.OpcUa.Client/Application/Connection/DefaultReconnectStrategy.cs:28` — unused `_` exception variable (CS0168).
- Obsolete API usage across `CertificateBasedSecurityConfigurationFactory.cs:60`, `ServerApplication.cs:10` (CS0618) — OPC Foundation moved to `ITelemetryContext`-based constructors; address when upgrading the SDK.

## Auto-generated suspect

### BUG-007 — `dotnet test` reports "Zero tests ran" (exit code 5) on the integration project
- This is tooling, not product code: the MTP runner (see `global.json` + `.opencode/tunit-docs.md`) miscounts discovery under `dotnet test`. Reliable path is running the built test app directly. Kept here so it is not re-diagnosed as a product bug.
