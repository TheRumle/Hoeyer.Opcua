using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity;

public interface IEntityNodeManagerFactory
{
    IEnumerable<INodeManager> GetEntityManagers(IServerInternal server, ApplicationConfiguration configuration);
}

internal sealed class EntityNodeManagerFactory(IEnumerable<IEntityObjectStateCreator> entityObjectCreators) : IEntityNodeManagerFactory
{
    public IEnumerable<INodeManager> GetEntityManagers(IServerInternal server, ApplicationConfiguration configuration)
    {
        return entityObjectCreators.Select(entityObjectCreator =>
            new SingletonEntityNodeManager(entityObjectCreator, server, configuration));
    }
}