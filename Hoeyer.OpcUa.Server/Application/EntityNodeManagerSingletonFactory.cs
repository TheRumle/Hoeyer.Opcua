using System.Threading.Tasks;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;


[OpcUaEntityService(typeof(IEntityNodeManagerFactory), ServiceLifetime.Singleton)]
[OpcUaEntityService(typeof(IEntityNodeManagerFactory<>), ServiceLifetime.Singleton)]
internal sealed class EntityNodeManagerSingletonFactory<T>(
    ILoggerFactory factory,
    IManagedEntityNodeSingletonFactory<T> nodeFactory,
    IEntityNodeAccessConfigurator configurator) : IEntityNodeManagerFactory<T>
{
    public async Task<IEntityNodeManager> CreateEntityManager(IServerInternal server)
    {
        if (CreatedManager != null) return CreatedManager;
        var node = await nodeFactory.CreateManagedEntityNode(server.NamespaceUris.GetIndexOrAppend);
        configurator.ConfigureAccess(node);
        var logger = factory.CreateLogger(node.BaseObject.BrowseName.Name+"Manager");
        CreatedManager = new EntityNodeManager<T>(node, server, logger);
        return CreatedManager;
    }
    public IEntityNodeManager<T> CreatedManager { get; private set; }
}