using System.Globalization;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration;

public static class WebProtocolExtensions
{
    public static string ToOpcUriScheme(this WebProtocol webProtocol)
    {
        return webProtocol switch
        {
            WebProtocol.OpcTcp => Utils.UriSchemeOpcTcp,
            WebProtocol.Https => Utils.UriSchemeHttps,
            WebProtocol.WebSocketSecure => Utils.UriSchemeOpcWss,
            var _ => throw new ArgumentOutOfRangeException(nameof(webProtocol), webProtocol,
                "Webprotocol is an unsupported scheme")
        };
    }

    public static Uri ToUri(this WebProtocol webProtocol, string host, int port)
    {
        var uriString = $"{webProtocol.ToOpcUriScheme()}://{host}:{port}";
        return ParseUri(uriString);
    }

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
}