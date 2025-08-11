using System.Threading.Tasks;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IEntityNodeManagerFactory<in T> : IEntityNodeManagerFactory;

public interface IEntityNodeManagerFactory
{
    Task<IEntityNodeManager> CreateEntityManager(IServerInternal server);
}