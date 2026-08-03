namespace Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

public interface IApplicationTargetConfigurationBuilder
{
    /// <summary>
    /// Sets the target server ID to <paramref name="serverId"/>.
    /// </summary>
    /// <param name="serverId">The ID of the server</param>
    /// <returns></returns>
    IServerNameStep WithServerId(string serverId);
}