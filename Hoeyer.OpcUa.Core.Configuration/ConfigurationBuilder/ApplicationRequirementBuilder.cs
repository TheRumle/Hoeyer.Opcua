using System.Globalization;

namespace Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

public class ApplicationRequirementBuilder : IApplicationTargetConfigurationBuilder, IServerNameStep, IWithOriginsStep,
    IWithApplicationUri, IWithSecurityConfiguration, IApplicationTargetConfigurationBuildable
{
    private string _applicationUriString = string.Empty;
    private string _hostString = string.Empty;
    private int _port;
    private string _serverId = string.Empty;
    private string _serverName = string.Empty;
    private WebProtocol _webProtocol;
    private CertificateConfiguration? _security = null;


    private ApplicationRequirementBuilder()
    {
    }

    public IApplicationConfigurationRequirements Build()
    {
        var host = _webProtocol.ToUri(_hostString, _port);
        var applicationUri = ParseUri(host + ParseAdditionalPath(_applicationUriString));
        if (!host.IsBaseOf(applicationUri))
        {
            throw new ArgumentException($"The host uri {host} is not a base of application {applicationUri}");
        }

        return new ApplicationConfigurationRequirements(_serverId, _serverName, host, applicationUri, _security);
    }

    public IServerNameStep WithServerId(string serverId)
    {
        _serverId = serverId;
        return this;
    }

    public IWithOriginsStep WithServerName(string serverName)
    {
        _serverName = serverName;
        return this;
    }

    /// <inheritdoc />
    public IWithSecurityConfiguration WithApplicationUri(string applicationNameUri)
    {
        _applicationUriString = applicationNameUri;
        return this;
    }

    /// <inheritdoc />
    public IWithApplicationUri WithWebOrigins(WebProtocol protocol, string host, int port)
    {
        _hostString = host;
        _webProtocol = protocol;
        _port = port;
        return this;
    }

    public IApplicationTargetConfigurationBuildable WithSecurityConfiguration(CertificateConfiguration options)
    {
        _security = options;
        return this;
    }

    public static IApplicationTargetConfigurationBuilder Create() => new ApplicationRequirementBuilder();

    private static Uri ParseUri(string uriString)
    {
        var isValidUri = Uri.TryCreate(string.Format(CultureInfo.InvariantCulture, "{0}", uriString),
            UriKind.RelativeOrAbsolute, out var result);

        if (!isValidUri)
        {
            throw new ArgumentException($"Host and serverId could not form a valid URI: {uriString}");
        }

        return result;
    }

    private static string ParseAdditionalPath(string additionalPath)
    {
        if (!additionalPath.Any())
        {
            return additionalPath;
        }

        if (additionalPath.Length > 1 && additionalPath.StartsWith('/'))
        {
            return additionalPath.Substring(1);
        }

        if (additionalPath.Length >= 1 && !additionalPath.StartsWith('/'))
        {
            return "/" + additionalPath;
        }

        return additionalPath;
    }

    public IApplicationTargetConfigurationBuildable WithDefaultSecurityConfiguration() =>
        throw new NotImplementedException();
}