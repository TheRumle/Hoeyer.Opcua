namespace Hoeyer.OpcUa.Core.Configuration.AgentServerBuilder;

public interface IServerNameStep
{
    /// <summary>
    /// The name of the OpcUa server the application will target
    /// </summary>
    /// <param name="serverName"> the name of the server</param>
    /// <returns></returns>
    IHostStep WithServerName(string serverName);
}