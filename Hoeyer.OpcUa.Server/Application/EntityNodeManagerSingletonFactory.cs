using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.Errors;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class EntityNodeManagerSingletonFactory<T>(
    IApplicationConfigurationRequirements info,
    ILoggerFactory loggerFactory,
    IManagedEntityNodeProvider<T> nodeProvider,
    IEnumerable<INodeConfigurator<T>> nodeConfigurators,
    IEntityNodeAccessConfigurator accessConfigurator)
    : IEntityNodeManagerFactory<T>, IEntityManagerHolder<T>
{
    public string EntityName { get; } = typeof(T).Name;
    public IEntityNodeManager? Manager { get; private set; }

    public IEntityNodeManager CreateEntityManager(IServerInternal server)
    {
        Manager ??= CreateManager(server);
        return Manager;
    }

    private EntityNodeManager<T> CreateManager(IServerInternal server)
    {
        var uriString = info.ApplicationNamespace + $"/{typeof(T).Name}";
        if (!Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
        {
            throw new InvalidUaConfigurationException(uriString + " is not a valid URI");
        }

        var logger = loggerFactory.CreateLogger<EntityNodeManager<T>>();
        var manager = new EntityNodeManager<T>(
            uri,
            nodeProvider,
            nodeConfigurators,
            logger,
            accessConfigurator,
            server
        );
        Manager = manager;
        return manager;
    }
}