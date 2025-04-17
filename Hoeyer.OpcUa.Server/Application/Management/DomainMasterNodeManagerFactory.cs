using System;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Entity.Management;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;

internal sealed class DomainMasterNodeManagerFactory(
    IEntityNodeManagerFactory entityManagerFactory,
    IOpcUaEntityServerInfo info
) : IDomainMasterManagerFactory
{
    private readonly Uri _host = info.Host;

    /// <inheritdoc />
    public DomainMasterNodeManager ConstructMasterManager(IServerInternal server,
        ApplicationConfiguration applicationConfiguration)
    {
        Func<string, (string @namespace, ushort index)> namespaceCreation = entityName =>
        {
            var nodeNamespace = _host.Host + $"/{entityName}";
            var namespaceIndex = server.NamespaceUris.GetIndexOrAppend(nodeNamespace);
            return (nodeNamespace, namespaceIndex);
        };
        
        var additionalManagers = entityManagerFactory.CreateEntityManagers(namespaceCreation, server).Result;
        return new DomainMasterNodeManager(server, applicationConfiguration, additionalManagers.ToArray());
    }
}