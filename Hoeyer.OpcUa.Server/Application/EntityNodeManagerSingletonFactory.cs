using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.Errors;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class EntityNodeManagerSingletonFactory<T>(
    IOpcUaTargetServerInfo info,
    IServiceProvider serviceProvider,
    MaybeInitializedEntityManager<T> loadableManager) : IEntityNodeManagerFactory<T>
{
    public IEntityNodeManager<T>? CreatedManager { get; private set; }

    public async Task<IEntityNodeManager> CreateEntityManager(IServerInternal server)
    {
        CreatedManager ??= await CreateManager(server);
        return CreatedManager;
    }

    private async Task<EntityNodeManager<T>> CreateManager(IServerInternal server)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var asyncProvider = scope.ServiceProvider;
        var nodeProvider = asyncProvider.GetRequiredService<IManagedEntityNodeProvider<T>>();
        var configurators = asyncProvider.GetRequiredService<IEnumerable<INodeConfigurator<T>>>();
        var logger = asyncProvider.GetRequiredService<ILogger<EntityNodeManager<T>>>();
        var accessConfigurator = asyncProvider.GetRequiredService<IEntityNodeAccessConfigurator>();
        var uriString = info.ApplicationNamespace + $"/{typeof(T).Name}";
        if (!Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
        {
            throw new InvalidServerConfigurationException(uriString + " is not a valid URI");
        }

        var manager = new EntityNodeManager<T>(
            uri,
            nodeProvider,
            configurators,
            logger,
            accessConfigurator,
            server
        );
        loadableManager.Manager = manager;
        return manager;
    }
}