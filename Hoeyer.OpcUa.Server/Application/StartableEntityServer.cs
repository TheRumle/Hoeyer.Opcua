using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class StartableEntityServer : IDisposable
{
    public readonly ApplicationInstance ApplicationInstance;
    public readonly OpcEntityServer EntityServer;
    private bool _disposed;

    public StartableEntityServer(ApplicationInstance applicationInstance, OpcEntityServer entityServer)
    {
        ApplicationInstance = applicationInstance ?? throw new ArgumentNullException(nameof(applicationInstance));
        EntityServer = entityServer ?? throw new ArgumentNullException(nameof(entityServer));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<StartedEntityServer> StartAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(StartableEntityServer));
        await ApplicationInstance.Start(EntityServer);
        var l = new List<BaseInstanceState>();

        var gantry = EntityServer.EntityManager.ManagedEntities.First();
        gantry.Folder.GetChildren(EntityServer.Server.DefaultSystemContext, l);
        Console.WriteLine(gantry.Entity.BrowseName.NamespaceIndex);
        
        Console.WriteLine(string.Join(", ", l.Select(e=>e.BrowseName)));
        
        var managedEntity = EntityServer.EntityManager.ManagedEntities.First();
        var jsonString = JsonSerializer.Serialize(managedEntity);
        Console.WriteLine(jsonString);
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