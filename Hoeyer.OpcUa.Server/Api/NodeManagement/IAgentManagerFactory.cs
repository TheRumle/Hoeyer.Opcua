using System.Threading.Tasks;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IAgentManagerFactory<in T> : IAgentManagerFactory;

public interface IAgentManagerFactory
{
    Task<IAgentManager> CreateAgentManager(IServerInternal server);
}