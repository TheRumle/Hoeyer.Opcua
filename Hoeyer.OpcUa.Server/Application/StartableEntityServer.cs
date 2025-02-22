using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

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

    public void Stop()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(StartableEntityServer));
        EntityServer.Stop();
        ApplicationInstance.Stop();
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