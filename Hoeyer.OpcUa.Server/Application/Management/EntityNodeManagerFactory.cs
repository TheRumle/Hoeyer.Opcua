using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api.Management;
using Hoeyer.OpcUa.Server.Application.Handle;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Management;

internal sealed class EntityNodeManagerFactory(
    ILoggerFactory loggerFactory,
    IEnumerable<IEntityServiceContainer> initializers) : IEntityNodeManagerFactory
{
    public async Task<IEnumerable<IEntityNodeManager>> CreateEntityManagers(
        Func<string, (string @namespace, ushort index)> namespaceIndexFactory, IServerInternal server)
    {
        return await Task.WhenAll(initializers.Select(async initializer =>
        {
            var (@namespace, index) = namespaceIndexFactory.Invoke(initializer.EntityName);
            var node = await initializer.CreateNode(index);
            var publisher = initializer.EntityChangedPublisher;
            
            var managedNode = new ManagedEntityNode(node, @namespace, index);

            publisher.Publish(managedNode);
            var entityName = managedNode.BaseObject.DisplayName.Text;
            var logger = loggerFactory.CreateLogger(entityName + "Manager");
            
            return new EntityNodeManager(
                managedNode,
                server,
                new EntityHandler(managedNode),
                new EntityStateChanger(managedNode, publisher),
                new EntityBrowser(managedNode),
                new EntityReader(managedNode, new PropertyReader()),
                new EntityReferenceLinker(managedNode, logger),
                logger);
        }));
    }
}