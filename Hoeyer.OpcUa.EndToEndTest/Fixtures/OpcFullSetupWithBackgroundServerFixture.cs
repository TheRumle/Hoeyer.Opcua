using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Test.Fixtures;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Server.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class OpcFullSetupWithBackgroundServerFixture : OpcUaCoreServicesFixture
{
    public OpcFullSetupWithBackgroundServerFixture()
    {
        OnGoingOpcEntityServiceRegistration
            .WithOpcUaServer()
            .WithOpcUaClientServices()
            .WithOpcUaServerSimulation()
            .WithOpcUaServerAsBackgroundService();
    }

    public virtual IEnumerable<ServiceDescriptor> Services => ServiceCollection;

    public static implicit operator List<ServiceDescriptor>(
        OpcFullSetupWithBackgroundServerFixture servicesServerFixture) => servicesServerFixture.Services.ToList();
}