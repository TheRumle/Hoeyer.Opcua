using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Abstractions.NodeManagement;

public interface IEntityNodeManagerFactory<in T> : IEntityNodeManagerFactory;

public interface IEntityNodeManagerFactory
{
    IEntityNodeManager CreateEntityManager(IServerInternal server);
}