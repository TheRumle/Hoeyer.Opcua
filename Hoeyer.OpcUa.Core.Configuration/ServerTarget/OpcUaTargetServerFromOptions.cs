using Hoeyer.OpcUa.Core.Configuration.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hoeyer.OpcUa.Core.Configuration.ServerTarget;

internal class OpcUaTargetServerFromOptions(
    IOptions<OpcUaOptions> optionsWrapper,
    ILogger<OpcUaTargetServerFromOptions> optionsFactoryLogger) : IOpcUaTargetServerInfoFactory
{
    public IOpcUaTargetServerInfo Get()
    {
        if (optionsWrapper?.Value == null)
        {
            throw new InvalidOperationException($"No {nameof(IOptions<OpcUaOptions>)} was configured");
        }

        var options = optionsWrapper.Value;
        optionsFactoryLogger.LogInformation("Using options {Options}", options);
        var applicationUri = options.ApplicationUri.StartsWith('/')
            ? options.ApplicationUri
            : $"/{options.ApplicationUri}";

        return EntityServerConfigurationBuilder.Create()
            .WithServerId(options.ServerId)
            .WithServerName(options.ServerName)
            .WithWebOrigins(options.Protocol, options.Host, options.Port)
            .WithApplicationUri(applicationUri)
            .Build();
    }
}