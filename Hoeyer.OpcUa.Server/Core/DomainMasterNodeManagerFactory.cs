using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Entity.Management;
using Hoeyer.OpcUa.Server.Exceptions;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Core;

internal sealed class DomainMasterNodeManagerFactory(
    EntityNodeManagerFactory entityManagerFactory,
    IEnumerable<IEntityInitializer> initializers,
    IOpcUaEntityServerInfo info
) : IDomainMasterManagerFactory
{
    /// <inheritdoc />
    public DomainMasterNodeManager ConstructMasterManager(IServerInternal server,
        ApplicationConfiguration applicationConfiguration)
    {
        var additionalManagers = initializers.Select(async e =>
        {
            var node = await CreatedManagedNode(server, info.Host, e);
            return Result.Ok(entityManagerFactory.Create(node, server));
        }).Traverse().Result;

        if (additionalManagers.IsFailed)
        {
            throw new ServerSetupException ("Something went wrong while constructing node managers for an entity: \n" + string.Join("\n", additionalManagers.Errors));
        }

        
        return new DomainMasterNodeManager(server, applicationConfiguration, additionalManagers.Value.ToArray());
    }
    
    private static async Task<ManagedEntityNode> CreatedManagedNode(
        IServerInternal server,
        Uri host,
        IEntityInitializer initializer)
    {
        var nodeNamespace = host.Host + $"/{initializer.EntityName}";
        var namespaceIndex = server.NamespaceUris.GetIndexOrAppend(nodeNamespace);
        var node = await initializer.CreateNode(namespaceIndex);
        return new ManagedEntityNode(node, nodeNamespace, namespaceIndex);
    }
}