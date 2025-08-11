using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration;

namespace Hoeyer.OpcUa.Server.Api;

public interface IStartableAgentServer
{
    IOpcUaAgentServerInfo ServerInfo { get; }
    Task<IStartedAgentServer> StartAsync();
}