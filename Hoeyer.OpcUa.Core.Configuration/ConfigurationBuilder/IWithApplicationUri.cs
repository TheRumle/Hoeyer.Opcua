namespace Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

public interface IWithApplicationUri
{
    /// <example>"/machines"</example>
    /// <example>"/data"</example>
    /// <example>"/domain"</example>
    /// <param name="applicationNameUri">a string describing where the application namespace origins from</param>
    IWithSecurityConfiguration WithApplicationUri(string applicationNameUri);
}