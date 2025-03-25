using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using NamespaceDoc = Opc.Ua.NamespaceDoc;

namespace Hoeyer.OpcUa.Server.Core;

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