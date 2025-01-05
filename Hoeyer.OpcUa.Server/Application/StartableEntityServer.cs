using System;
using System.Linq;
using System.Threading.Tasks;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class StartedEntityServer(StartableEntityServer server) : IDisposable
{
    private bool _isStopped = false;
    public void Stop()
    {
        server.Stop();
        _isStopped = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_isStopped)
        { 
            Stop();
        }
        server.Dispose();
    }
}
public sealed class StartableEntityServer : IDisposable
{
    private bool _disposed;
    public readonly ApplicationInstance ApplicationInstance;
    public readonly OpcEntityServer EntityServer;

    public StartableEntityServer(ApplicationInstance applicationInstance, OpcEntityServer entityServer)
    {
        ApplicationInstance = applicationInstance ?? throw new ArgumentNullException(nameof(applicationInstance));
        EntityServer = entityServer ?? throw new ArgumentNullException(nameof(entityServer));
    }

    public async Task<StartedEntityServer> StartAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(StartableEntityServer));

        await ApplicationInstance.Start(EntityServer);
        var jsonString = System.Text.Json.JsonSerializer.Serialize(EntityServer.EntityNodes.First());
        
        return new (this);
    }

    public void Stop()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(StartableEntityServer));
        EntityServer.Stop();
        ApplicationInstance.Stop();
    }
    
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
            EntityServer.Stop();
            ApplicationInstance?.Stop();
            EntityServer?.Dispose();
        }
        _disposed = true;
    }

    ~StartableEntityServer()
    {
        Dispose(false);
    }
}