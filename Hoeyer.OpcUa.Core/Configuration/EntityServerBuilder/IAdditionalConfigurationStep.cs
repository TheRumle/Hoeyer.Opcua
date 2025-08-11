using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.AgentServerBuilder;

public interface IAdditionalConfigurationStep : IAgentServerConfigurationBuildable
{
    public IAgentServerConfigurationBuildable WithAdditionalConfiguration(
        Action<ServerConfiguration> additionalConfigurations);
}