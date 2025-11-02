using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Services.Configuration;

namespace Hoeyer.OpcUa.Server.Api;

public interface IStartableEntityServer
{
    IOpcUaTargetServerSetup ServerInfo { get; }
    Task<IStartedEntityServer> StartAsync();
}