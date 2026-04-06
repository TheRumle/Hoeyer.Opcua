using System.Linq;
using System.Text.Json;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions.Logging;

public static class LoggingExtensions
{
    public static object ToLoggingObject(this ApplicationConfiguration configuration)
    {
        return Format(new
        {
            configuration.ApplicationName,
            configuration.ProductUri,
            configuration.Properties,
            Extensions = configuration.ExtensionObjects,
            Other = new
            {
                DomainNames = configuration.GetServerDomainNames(),
            },
            Security = new
            {
                configuration.SecurityConfiguration?.SupportedSecurityPolicies
            },
            KnownDiscoveryUrls = configuration.ClientConfiguration?.WellKnownDiscoveryUrls?.Select(e => e).ToArray()
        });
    }

    private static string Format(object obj) => JsonSerializer.Serialize(obj);
}