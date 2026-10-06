namespace Hoeyer.OpcUa.Fixtures.Common;

/// <summary>
///     Hands a session-owned service to a test without transferring ownership of it.
///     <para>
///         TUnit treats an injected <see cref="IDisposable" /> or <see cref="IAsyncDisposable" /> member as test-owned
///         and disposes it once that test finishes. For a service that is shared by the whole session - the OPC UA
///         server above all - that silently tears down the server for every test that runs afterwards.
///     </para>
///     <para>
///         Wrapping the service in <see cref="NonOwned{T}" /> makes the non-ownership explicit at the injection
///         boundary and, because this type is not disposable, keeps the wrapped instance out of the runner's
///         disposal set. The wrapper is a transparent forwarder, so the underlying service stays usable.
///     </para>
/// </summary>
/// <typeparam name="T">The borrowed service type.</typeparam>
public sealed class NonOwned<T>(T value) where T : class
{
    /// <summary>
    ///     The borrowed instance. Do not dispose it; its lifetime belongs to the test environment.
    /// </summary>
    public T Value { get; } = value;
}