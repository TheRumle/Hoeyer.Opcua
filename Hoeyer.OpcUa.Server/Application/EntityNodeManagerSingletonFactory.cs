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
    MaybeInitializedEntityManager<T> loadableManager,
    IEntityNodeAccessConfigurator configurator) : IEntityNodeManagerFactory<T>
{
    public IEntityNodeManager<T>? CreatedManager { get; private set; }

    public async Task<IEntityNodeManager> CreateEntityManager(IServerInternal server)
    {
        CreatedManager ??= await CreateManager(server);
        return CreatedManager;
    }

    private async Task<EntityNodeManager<T>> CreateManager(IServerInternal server)
    {
        var node = await nodeFactory.CreateManagedEntityNode(server.NamespaceUris.GetIndexOrAppend);
        configurator.Configure(node);
        ILogger logger = factory.CreateLogger(node.BaseObject.BrowseName.Name + "Manager");
        var createdManager = new EntityNodeManager<T>(node, server, logger);

        loadableManager.Manager = createdManager;
        return createdManager;
    }
}