using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public record OnGoingOpcEntityServerServiceRegistration(IServiceCollection Collection)
    : OnGoingOpcEntityServiceRegistration(Collection)
{
}