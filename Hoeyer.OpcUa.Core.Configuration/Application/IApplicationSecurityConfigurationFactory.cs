using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.Application;

public interface IApplicationSecurityConfigurationFactory
{
    SecurityConfiguration Configure(ApplicationConfiguration configuration);
}