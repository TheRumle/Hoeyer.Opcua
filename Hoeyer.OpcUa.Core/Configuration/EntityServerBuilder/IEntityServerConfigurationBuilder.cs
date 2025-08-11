namespace Hoeyer.OpcUa.Core.Configuration.AgentServerBuilder;

public interface IAgentServerConfigurationBuilder
{
    /// <summary>
    /// Sets the target server ID to <paramref name="serverId"/>.
    /// </summary>
    /// <param name="serverId">The ID of the server</param>
    /// <returns></returns>
    IServerNameStep WithServerId(string serverId);
}