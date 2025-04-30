using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class AllOpcUaServicesFixture : OpcUaCoreServicesFixture
{
    public static implicit operator List<ServiceDescriptor>(AllOpcUaServicesFixture servicesFixture) => servicesFixture.Services.ToList();
    
    public virtual IEnumerable<ServiceDescriptor> Services => ServiceCollection;

    public AllOpcUaServicesFixture()
    {
        OnGoingOpcEntityServiceRegistration
            .WithOpcUaServer()
            .WithOpcUaClientServices();
    }
}