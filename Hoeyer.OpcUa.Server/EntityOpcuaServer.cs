using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;
public sealed class OpcEntityServerDriver : IDisposable
{
    private bool _disposed;
    public readonly ApplicationInstance ApplicationInstance;
    public readonly OpcEntityServer EntityServer;

    public OpcEntityServerDriver(ApplicationInstance applicationInstance, OpcEntityServer entityServer)
    {
        ApplicationInstance = applicationInstance ?? throw new ArgumentNullException(nameof(applicationInstance));
        EntityServer = entityServer ?? throw new ArgumentNullException(nameof(entityServer));
    }

    public async Task<Uri> StartAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OpcEntityServerDriver));

        try
        {
            await ApplicationInstance.Start(EntityServer);
            return EntityServer.RootUri;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Stop()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OpcEntityServerDriver));

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
            EntityServer?.Stop();
            ApplicationInstance?.Stop();
            EntityServer?.Dispose();
        }
        _disposed = true;
    }

    ~OpcEntityServerDriver()
    {
        Dispose(false);
    }
}