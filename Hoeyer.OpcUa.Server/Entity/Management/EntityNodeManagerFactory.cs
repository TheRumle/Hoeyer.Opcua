using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Application;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Management;

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
            (IEntityNode node, IMessagePublisher<IEntityNode> publisher) = await initializer.CreateNode(index);
            var managedNode = new ManagedEntityNode(node, @namespace, index);

            publisher.Publish(managedNode);
            var entityName = managedNode.BaseObject.DisplayName.Text;
            var logger = loggerFactory.CreateLogger(entityName + "Manager");
            
            return new EntityNodeManager(
                managedNode,
                server,
                new EntityHandleManager(managedNode),
                new EntityStateChanger(managedNode, publisher),
                new EntityBrowser(managedNode),
                new EntityReader(managedNode, new PropertyReader()),
                new EntityReferenceLinker(managedNode, logger),
                logger);
        }));
    }
}