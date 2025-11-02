namespace Hoeyer.OpcUa.Core.Configuration.ServerTarget;

public interface IWithApplicationUri : IEntityServerConfigurationBuildable
{
    /// <example>"/machines"</example>
    /// <example>"/data"</example>
    /// <example>"/domain"</example>
    /// <param name="applicationNameUri">a string describing where the application namespace origins from</param>
    IWithApplicationUri WithApplicationUri(string applicationNameUri);
}