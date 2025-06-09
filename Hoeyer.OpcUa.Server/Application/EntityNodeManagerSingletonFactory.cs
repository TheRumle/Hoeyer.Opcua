using System;
using System.Collections.Generic;
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
    IEnumerable<IPreinitializedNodeConfigurator<T>> preinitializedNodeConfigurators,
    IEntityNodeAccessConfigurator accessConfigurator) : IEntityNodeManagerFactory<T>
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
        List<Exception> configurationExceptions = ConfigurePreInitialization(node);
        if (configurationExceptions.Count > 0) throw new AggregateException(configurationExceptions);


        ILogger logger = factory.CreateLogger(node.BaseObject.BrowseName.Name + "Manager");
        var createdManager = new EntityNodeManager<T>(node, server, logger);
        loadableManager.Manager = createdManager; //mark the manager being loaded

        return createdManager;
    }

    private List<Exception> ConfigurePreInitialization(IManagedEntityNode node)
    {
        var exceptions = new List<Exception>();
        accessConfigurator.Configure(node);
        foreach (IPreinitializedNodeConfigurator<T>? configurator in preinitializedNodeConfigurators)
        {
            try
            {
                configurator.Configure(node);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }

        return exceptions;
    }
}