using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration;

namespace Hoeyer.OpcUa.Server.Api;

public interface IStartableEntityServer
{
    IOpcUaTargetServerInfo ServerInfo { get; }
    Task<IStartedEntityServer> StartAsync();
}