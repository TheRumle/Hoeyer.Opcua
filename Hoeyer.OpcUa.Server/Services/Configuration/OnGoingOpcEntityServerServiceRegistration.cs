using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Services.Configuration;

public record OnGoingOpcAgentServerServiceRegistration(IServiceCollection Collection)
    : OnGoingOpcAgentServiceRegistration(Collection)
{
}