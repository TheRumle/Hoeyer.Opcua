using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Management;

internal interface IEntityNodeManagerFactory
{
    Task<IEnumerable<IEntityNodeManager>> CreateEntityManagers(
        Func<string, (string @namespace, ushort index)> namespaceIndexFactory, IServerInternal server);
}

internal sealed class EntityNodeManagerFactory(
    ILoggerFactory loggerFactory,
    IEnumerable<IEntityInitializer> initializers) : IEntityNodeManagerFactory
{
    public async Task<IEnumerable<IEntityNodeManager>> CreateEntityManagers(
        Func<string, (string @namespace, ushort index)> namespaceIndexFactory, IServerInternal server)
    {
        return await Task.WhenAll(initializers.Select(async initializer =>
        {
            var (@namespace, index) = namespaceIndexFactory.Invoke(initializer.EntityName);
            var node = await initializer.CreateNode(index);
            var managedNode = new ManagedEntityNode(node, @namespace, index);

            var entityName = managedNode.BaseObject.DisplayName.Text;
            var logger = loggerFactory.CreateLogger(entityName + "Manager");
            return new EntityNodeManager(
                managedNode,
                server,
                new EntityHandleManager(managedNode),
                new EntityWriter(managedNode),
                new EntityBrowser(managedNode),
                new EntityReader(managedNode, new PropertyReader()),
                new EntityReferenceLinker(managedNode, logger),
                logger);
        }));
    }
}