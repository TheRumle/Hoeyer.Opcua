using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Test.Fixtures;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class OpcFullSetupWithBackgroundServerFixture : OpcUaCoreServicesFixture
{
    public OpcFullSetupWithBackgroundServerFixture()
    {
        OnGoingOpcAgentServiceRegistration
            .WithOpcUaServer()
            .WithOpcUaClientServices()
            .WithOpcUaSimulationServices(configure =>
            {
                configure
                    .WithTimeScaling(double.Epsilon)
                    .AdaptToRuntime<ServerSimulationAdapter>();
            })
            .WithOpcUaServerAsBackgroundService()
            .Collection.AddLogging(e => e.AddSimpleConsole());
    }

    public virtual IEnumerable<ServiceDescriptor> Services => ServiceCollection;

    public static implicit operator List<ServiceDescriptor>(
        OpcFullSetupWithBackgroundServerFixture servicesServerFixture) => servicesServerFixture.Services.ToList();
}