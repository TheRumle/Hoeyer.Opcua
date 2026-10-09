using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test;

[ServiceInjection]
public class ApplicationConfigurationTest(
    IServerApplicationConfigurationFactory serverConfigFactory)
{
    [Test]
    [DisplayName("Server configuration must be validatable")]
    public async Task ValidationForServerMustSucceed(CancellationToken cancellationToken = default)
    {
        var clientConfiguration = serverConfigFactory.CreateServerConfiguration();
        await clientConfiguration.ValidateAsync(ApplicationType.Server, cancellationToken);
    }
}