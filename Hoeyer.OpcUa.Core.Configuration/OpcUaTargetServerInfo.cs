using Hoeyer.OpcUa.Core.Configuration.Errors;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration;

internal record OpcUaTargetServerInfo : IOpcUaTargetServerInfo
{
    public OpcUaTargetServerInfo(string serverId, string serverName, Uri host, Uri applicationNamespace)
    {
        ValidateSupportedProtocol([host, applicationNamespace]);
        Host = host;
        ApplicationNamespace = applicationNamespace;
        ServerId = serverId;
        ApplicationName = serverName;
    }

    public Uri Host { get; }

    public string ServerId { get; }
    public string ApplicationName { get; }

    /// <summary>
    ///     For instance, http://samples.org/UA/MyApplication or something else uniquely identifying the overall resource,
    /// </summary>
    public Uri ApplicationNamespace { get; }

    private static void ValidateSupportedProtocol(IEnumerable<Uri> addresses)
    {
        var errors = addresses.Select(address => address.Scheme switch
            {
                Utils.UriSchemeHttps or Utils.UriSchemeOpcHttps => null,
                Utils.UriSchemeOpcTcp => null,
                Utils.UriSchemeOpcWss => null,
                "http" => "HTTP is not supported, use https instead",
                var _ => $"Protocol {address.Scheme} is not supported"
            })
            .Where(e => e is not null)
            .ToArray();

        if (errors.Length > 0) throw new InvalidServerConfigurationException(string.Join("\n", errors));
    }
}