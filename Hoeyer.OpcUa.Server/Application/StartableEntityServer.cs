using System;
using System.Threading.Tasks;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class StartableEntityServer(ApplicationInstance applicationInstance, OpcEntityServer entityServer)
    : IDisposable
{
    public readonly ApplicationInstance ApplicationInstance = applicationInstance ?? throw new ArgumentNullException(nameof(applicationInstance));
    public readonly OpcEntityServer EntityServer = entityServer ?? throw new ArgumentNullException(nameof(entityServer));
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<StartedEntityServer> StartAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(StartableEntityServer));
        await ApplicationInstance.Start(EntityServer);
        return new StartedEntityServer(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            EntityServer.Stop();
            ApplicationInstance.Stop();
            EntityServer.Dispose();
        }

        _disposed = true;
    }

    ~StartableEntityServer()
    {
        Dispose(false);
    }
}