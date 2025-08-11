using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Configuration.AgentServerBuilder;

public interface IEndpointsStep : IAgentServerConfigurationBuildable
{
    IAgentServerConfigurationBuildable WithEndpoints(List<string> endpoints);
}