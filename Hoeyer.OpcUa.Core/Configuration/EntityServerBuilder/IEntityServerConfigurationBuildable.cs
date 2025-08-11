namespace Hoeyer.OpcUa.Core.Configuration.AgentServerBuilder;

public interface IAgentServerConfigurationBuildable
{
    IOpcUaAgentServerInfo Build();
}