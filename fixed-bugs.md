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
  remaining 4 failures are BUG-003 (fixed separately below).

## BUG-003 — The shared session server was disposed mid-run by the test runner — FIXED 2026-09-27

- Location: `Hoeyer.OpcUa.ClientServer.IntegrationTest/Configuration/ManagerHolderTests.cs`
  (root cause), plus the ownership fixes in `IntegrationFixtureResources.cs`,
  `LocalHostedIntegrationTestEnvironment.cs`, `IntegrationTestAdapter.cs` and the new
  `EnvironmentAdapter/DisposeCachedEnvironments.cs`.
- Symptom: session opens failed with `SocketException (10061) ... actively refused it` from
  `TcpMessageSocket.ConnectAsync` (earlier manifests: `BadRequestTimeout` `[80850000]`, and
  `[Timeout]` in `EntitySessionFactory`). It looked like an opening-wave latency/warm-up problem and
  was flaky/port-dependent, so several plausible-but-wrong theories were chased first.
- How it was actually diagnosed (all measured, not inferred):
    - An in-harness accept probe showed the server stopped accepting new TCP connections a few
      seconds after start and stayed dead. `netstat` polling showed the listener socket stayed bound
      (`LISTENING`) the whole run, i.e. the socket was not closed — so a mid-run teardown had to be
      identified rather than a warm-up/timing problem.
    - `AppDomain.CurrentDomain.FirstChanceException` surfaced
      `ObjectDisposedException: 'System.Threading.SemaphoreSlim'` — the
      `MasterNodeManager` startup/shutdown semaphore.
    - Logging `OpcEntityServer.Dispose` with a stack trace gave the decisive frame chain:
      `TUnit.Core.Tracking.ObjectTracker.UntrackObjects` → `Disposer.DisposeAsync` →
      `ServerBase.Dispose()` → `OpcEntityServer.Dispose` (with `IsServerStarted == true`).
- Root cause: **two** lifetime bugs, not one.
    1. `ManagerHolderTests` declared `IOpcEntityServer` as a *test-injected member*.
       `IntegrationServiceInjectionAttribute` resolves injected members from the session-owned root
       provider, and TUnit's `ObjectTracker` treats injected `IDisposable`/`IAsyncDisposable` values
       as test-owned — so it disposed the one shared OPC UA server when that test finished, killing it
       for every test that ran afterwards. Confirmed by logging the resolution:
       `member=IOpcEntityServer -> resolved=OpcEntityServer ... sameAsRootServer=True`.
    2. Every class fixture wrapped the *same* session-isolated environment and
       `IntegrationFixtureResources.DisposeAsync` disposed it, so the first finishing class tore down
       the server for all others. This contradicted the intent already documented on
       `IntegrationServiceInjectionAttribute` ("The environment itself is session-owned and must not be
       disposed by this data source"). It also produced the `NullReferenceException` at
       `IntegrationFixtureResources.DisposeAsync` for fixtures disposed without initializing.
- Fix:
    - `ManagerHolderTests` now injects `IServiceProvider` and resolves `IOpcEntityServer` *inside* the
      test body, so the shared server never enters TUnit's tracked injection graph. Test intent is
      unchanged.
    - `IntegrationServiceInjectionAttribute.Create` now **fails fast** if an injected member resolves
      to a session-owned disposable from the server assembly, with a message explaining the hazard and
      the `IServiceProvider` remedy — so this cannot regress silently.
    - `IntegrationFixtureResources.DisposeAsync` disposes only its own scope, never the shared
      environment; the session-owned environments are disposed once at session end via
      `IntegrationTestAdapter.DisposeCachedEnvironmentsAsync` driven by `[After(TestSession)]`
      (`EnvironmentAdapter/DisposeCachedEnvironments.cs`).
    - `LocalHostedIntegrationTestEnvironment` lifecycle hardened: nullable provider/health check,
      `AvailableServices` throws a clear error before init instead of `NullReferenceException`,
      `EnvironmentReady()` returns `false` instead of throwing, locked dispose that always releases the
      lock and permits re-init, and `GetFreeLoopbackPort()` now *stops* its probe `TcpListener`
      (previously the port stayed reserved between probing and binding).
    - `LocalHostedIntegrationTestEnvironment` now advertises `IPAddress.Loopback` (`127.0.0.1`) instead
      of the name `localhost`. On this machine `localhost` resolves to `::1` **first**, while the OPC
      SDK server binds IPv4 — so the client could dial `[::1]` and be refused. This was a real
      address-family mismatch, independent of BUG-003's teardown, and worth fixing on its own.
- Verification: `Hoeyer.OpcUa.slnx` builds with 0 errors. Integration run on 2026-09-27: **184 tests,
  184 passed, 0 failed, 0 skipped**, reproduced three times (default parallel, `--maximum-parallel-
  tests 1`, and a repeat parallel run). The filter that previously passed in isolation still passes,
  and the 34 dependency-skipped tests now run and pass.

## Notes

- Verified against `Hoeyer.OpcUa.ClientServer.IntegrationTest` runs on 2026-09-27
  (184 tests: baseline 149 ok / 1 failed / 34 skipped → post-fix 184 ok / 0 failed / 0 skipped).
- Earlier baselines for context: 2026-09-19 (143 ok / 4 failed / 37 skipped) and 2026-09-26
  (147 ok / 4 failed / 31 skipped), both with BUG-003 open.
- Lesson recorded deliberately: a flaky, port-dependent symptom in a *shared-fixture* integration
  harness should be bisected with an in-harness probe (does the server still accept? is the socket
  still bound?) before hypothesising about latency, backlog, or warm-up. Two of the three
  "fix directions" originally proposed for BUG-003 were wrong.
