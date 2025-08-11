using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaEntityService(typeof(IAgentManagerFactory), ServiceLifetime.Singleton)]
[OpcUaEntityService(typeof(IAgentManagerFactory<>), ServiceLifetime.Singleton)]
internal sealed class AgentManagerSingletonFactory<T>(
    ILoggerFactory factory,
    IManagedAgentSingletonFactory<T> nodeFactory,
    MaybeInitializedEntityManager<T> loadableManager,
    IEnumerable<INodeConfigurator<T>> preinitializedNodeConfigurators,
    IAgentAccessConfigurator accessConfigurator) : IAgentManagerFactory<T>
{
    public IAgentManager<T>? CreatedManager { get; private set; }

    public async Task<IAgentManager> CreateEntityManager(IServerInternal server)
    {
        CreatedManager ??= await CreateManager(server);
        return CreatedManager;
    }

    private async Task<AgentManager<T>> CreateManager(IServerInternal server)
    {
        IManagedAgent<T> node = await nodeFactory.CreateManagedAgent(server.NamespaceUris.GetIndexOrAppend);
        List<Exception> configurationExceptions = ConfigurePreInitialization(node, server.DefaultSystemContext);
        if (configurationExceptions.Count > 0)
        {
            throw new NodeSetupException(string.Join("\n", configurationExceptions.Select(e => e.Message)));
        }


        ILogger logger = factory.CreateLogger(node.Select(e => e.BaseObject.BrowseName.Name + "Manager"));
        var createdManager = new AgentManager<T>(node, server, logger);
        loadableManager.Manager = createdManager; //mark the manager being loaded

        return createdManager;
    }

    private List<Exception> ConfigurePreInitialization(IManagedAgent node, ISystemContext context)
    {
        var exceptions = new List<Exception>();
        accessConfigurator.Configure(node, context);
        foreach (INodeConfigurator<T>? configurator in preinitializedNodeConfigurators)
        {
            if (configurator == null) continue;
            try
            {
                configurator.Configure(node, context);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }

        return exceptions;
    }
}