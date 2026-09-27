# Known Bugs

Registry of known defects in this repo. Fix in order of severity. After fixing, re-run the
integration tests (see `.opencode/tunit-docs.md`) to verify, then move the entry to
`fixed-bugs.md`. Resolved defects (e.g. BUG-001, BUG-002, BUG-003) are archived there.
Evidence below is from the run on 2026-09-27 (`Hoeyer.OpcUa.ClientServer.IntegrationTest`,
184 tests: 184 ok, 0 failed, 0 skipped) after BUG-003 was fixed.

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
