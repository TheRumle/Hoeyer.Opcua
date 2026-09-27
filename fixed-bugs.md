# Fixed Bugs

Registry of resolved defects. Kept for reference; new (still-open) defects live in
`known-bugs.md`. Entries are archived here once their fix is verified.

## BUG-001 — `CreateAddressSpace` double-completes the readiness TCS — FIXED 2026-09-19

- Location: `Hoeyer.OpcUa.Server/Application/EntityNodeManager.cs` (+ new
  `Hoeyer.OpcUa.Server/DuplicateEntityNodeManagerSetupException.cs`).
- Symptom: `InvalidOperationException: An attempt was made to transition a task to a final
  state when it had already completed` from `_addressSpaceReady.SetResult` during server
  start. `CreateAddressSpace` was invoked more than once for the same manager, and
  `SetResult`/`SetException` (not `TrySet*`) throw on the second call.
- Impact: local server never started → 4 integration test failures + dependent skips; the
  root cause of the originally red test suite.
- Fix:
  - Guarded setup with `Interlocked.CompareExchange(ref _setupStarted, 1, 0)`; a duplicate
    setup now throws the typed `DuplicateEntityNodeManagerSetupException` instead of
    corrupting a completed TCS.
  - Completion uses `TrySetResult`/`TrySetException` (`HealthCheck.cs` pattern).
  - `_nodeTask` is now assigned in `CreateAddressSpace` (also cleared the BUG-005
    dead-field warnings).
- Verification: server project builds clean (0 errors); the OPC UA server starts and its
  endpoint is healthy (`The fixture must have a healthy environment` passes).

## Double-start of the test environment — FIXED 2026-09-19

- Location: `Hoeyer.OpcUa.ClientServer.IntegrationTest/EnvironmentAdapter/LocallyHostedServer/LocalHostedIntegrationTestEnvironment.cs`.
- Symptom: the session-isolated environment's `InitializeAsync` was invoked more than once —
  once by `IntegrationServiceInjectionAttribute` (via `GetSessionIsolatedAdapter()`) and once
  by each `[ClassDataSource<IntegrationTestFixture<T>>(Shared = PerTestSession)]` fixture —
  producing per-call fresh servers and leaking providers. This was BUG-001's trigger.
- Fix: `LocalHostedIntegrationTestEnvironment` is now single-shot and thread-safe:
  `SemaphoreSlim`-guarded `InitializeAsync`/`DisposeAsync` with `_initialized`/`_disposed`
  flags; `AvailableServices` returns one stable provider.
- Verification: no more duplicated server starts in traces; server starts exactly once per
  test session.

## BUG-002 — Disposing a failed/partially-started server throws on `MasterNodeManager.Dispose` — FIXED 2026-09-26

- Location: `Hoeyer.OpcUa.Server/OpcEntityServer.cs` (`Dispose(bool)`) +
  `Hoeyer.OpcUa.Server/Services/ServiceExtensions.cs` (health-check registration).
- Symptom: `System.ObjectDisposedException: Cannot access a disposed object. Object name:
  'System.Threading.SemaphoreSlim'` when tearing down after a failed start. Trace walked
  `StartableEntityServer.DisposeAsync → OpcEntityServer.Dispose → DomainManager?.Dispose →
  MasterNodeManager.Dispose → SemaphoreSlim.Wait`.
- Root cause: a failed/partial start already tears down the master node manager internals, so
  `OpcEntityServer.Dispose` disposing `DomainManager` a second time re-entered
  `MasterNodeManager.Dispose` and its `m_startupShutdownSemaphoreSlim.Wait()` threw on the
  already-disposed semaphore. This masked the original start error and added a dispose exception
  to every failed run.
- Fix:
  - `OpcEntityServer` now takes `IServerStartedHealthCheck` and only calls
    `DomainManager?.Dispose()` when `healthCheck.IsServerStarted`; otherwise it logs a warning and
    skips, leaving the already-torn-down internals alone. `base.Dispose` still runs.
  - Hardened the health-check registration: `AddServiceAndImplSingleton<IServerStartedHealthCheck,
    HealthCheck>()` created one `HealthCheck` per descriptor, and `IHealthCheckAssignment` mapped to
    the concrete type — so the interfaces could observe a *different* instance than the one
    `StartableEntityServer` marks. All three keys now resolve a single `HealthCheck` instance.
- Verification: solution builds with 0 errors; integration run on 2026-09-26 (182 tests: 147 ok /
  4 failed / 31 skipped) shows no `ObjectDisposedException`/`MasterNodeManager` teardown noise. The
  remaining 4 failures are BUG-003 (unchanged).

## Notes

- Verified against `Hoeyer.OpcUa.ClientServer.IntegrationTest` runs on 2026-09-19
  (184 tests: baseline 143 ok / 4 failed / 37 skipped → post-fix 149 ok / 4 failed /
  31 skipped; the remaining 4 are tracked as BUG-003 in `known-bugs.md` and are left open by
  decision of the session).