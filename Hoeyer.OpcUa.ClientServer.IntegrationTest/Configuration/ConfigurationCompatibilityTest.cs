using Hoeyer.OpcUa.Client.Abstractions.Configuration;
using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Hoeyer.OpcUa.Server.Test;
using Opc.Ua;

namespace Hoeyer.OpcUa.IntegrationTest.Configuration;

[Fixtures.DataSources.ServiceInjection]
public class ConfigurationCompatibilityTest(
    IClientApplicationConfigurationFactory clientConfigFactory,
    IServerApplicationConfigurationFactory serverConfigFactory)
{
    [Test]
    [DependsOn(nameof(ValidationForClientMustSucceed))]
    [DependsOn<ApplicationConfigurationTest>(nameof(ApplicationConfigurationTest.ValidationForServerMustSucceed))]
    [DisplayName("Client and server configuration must have same application details")]
    public async Task ClientServerConfigurationEquivalence()
    {
        var clientConfiguration = clientConfigFactory.CreateClientConfiguration();
        var serverConfiguration = serverConfigFactory.CreateServerConfiguration();

        await Assert.That(clientConfiguration.ApplicationName).IsEqualTo(serverConfiguration.ApplicationName)
            .Because("ApplicationName should be the same as in the client configuration.");
        await Assert.That(clientConfiguration.ApplicationUri).IsEqualTo(serverConfiguration.ApplicationUri)
            .Because("ApplicationUri should be the same as in the client configuration.");
        await Assert.That(clientConfiguration.ProductUri).IsEqualTo(serverConfiguration.ProductUri)
            .Because("ProductUri should be the same as in the client configuration.");
    }

    [Test]
    [DisplayName("Client configuration must be validatable")]
    public async Task ValidationForClientMustSucceed(CancellationToken cancellationToken = default)
    {
        var clientConfiguration = clientConfigFactory.CreateClientConfiguration();
        await clientConfiguration.ValidateAsync(ApplicationType.Client, cancellationToken);
    }


}