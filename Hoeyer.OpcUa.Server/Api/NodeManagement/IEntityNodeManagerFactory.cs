using System.Threading.Tasks;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

internal interface IEntityNodeManagerFactory<in T> : IEntityNodeManagerFactory;
internal interface IEntityNodeManagerFactory
{
    Task<IEntityNodeManager> CreateEntityManager(IServerInternal server);
}