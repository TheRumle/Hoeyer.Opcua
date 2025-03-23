using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Core;

internal sealed class StartableEntityServer(ApplicationInstance applicationInstance, OpcEntityServer entityServer)
    : IStartableEntityServer, IStartedEntityServer
{
    private readonly ApplicationInstance _applicationInstance =
        applicationInstance ?? throw new ArgumentNullException(nameof(applicationInstance));

    private readonly OpcEntityServer _entityServer =
        entityServer ?? throw new ArgumentNullException(nameof(entityServer));

    private bool _disposed;

    public async Task<IStartedEntityServer> StartAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(StartableEntityServer));
        await _applicationInstance.Start(_entityServer);
        return this;
    }

    /// <inheritdoc />
    public IOpcUaEntityServerInfo ServerInfo => _entityServer.ServerInfo;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _entityServer.Stop();
            _applicationInstance.Stop();
            _entityServer.Dispose();
        }

        _disposed = true;
    }

    ~StartableEntityServer()
    {
        Dispose(false);
    }
}