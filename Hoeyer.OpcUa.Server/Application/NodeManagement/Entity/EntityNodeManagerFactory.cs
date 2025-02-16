using Hoeyer.OpcUa.Entity;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.NodeManagement.Entity;

public sealed class EntityNodeManagerFactory(ILoggerFactory loggerFactory)
{
    internal EntityNodeManager Create(IServerInternal server, ApplicationConfiguration configuration,  IEntityNodeCreator nodeCreator)
    {
        var nodeNamespace = configuration.ApplicationUri + $"/Manager/{nodeCreator.EntityName}";
        var logger = loggerFactory.CreateLogger(nodeCreator.EntityName + nameof(EntityNodeManager));
        
        return new EntityNodeManager(
            nodeNamespace,
            nodeCreator,
            server,
            logger);
    }
    
}