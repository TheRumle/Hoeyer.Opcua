using System.Threading.Tasks;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;


[OpcUaEntityService(typeof(IEntityNodeManagerFactory), ServiceLifetime.Singleton)]
[OpcUaEntityService(typeof(IEntityNodeManagerFactory<>), ServiceLifetime.Singleton)]
internal sealed class EntityNodeManagerSingletonFactory<T>(
    IManagedEntityNodeSingletonFactory<T> nodeFactory,
    IEntityNodeAccessConfigurator configurator) : IEntityNodeManagerFactory<T>
{
    public async Task<IEntityNodeManager> CreateEntityManager(IServerInternal server)
    {
        if (CreatedManager != null) return CreatedManager;
        var node = await nodeFactory.CreateManagedEntityNode(server.NamespaceUris.GetIndexOrAppend);
        configurator.ConfigureAccess(node);
        CreatedManager = new EntityNodeManager<T>(node, server);
        return CreatedManager;
    }
    public IEntityNodeManager<T> CreatedManager { get; private set; }
}