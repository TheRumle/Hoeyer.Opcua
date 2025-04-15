using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class OpcUaServiceDescriptorFixture
{
    public static implicit operator List<ServiceDescriptor>(OpcUaServiceDescriptorFixture serviceDescriptorFixture) => serviceDescriptorFixture.Services.ToList();
    
    protected readonly ServiceCollection ServiceCollection;
    public virtual IEnumerable<ServiceDescriptor> Services => ServiceCollection;

    public OpcUaServiceDescriptorFixture()
    {
        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddOpcUaServerConfiguration(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithHttpsHost("localhost", 10)
                .WithEndpoints(["opc.tcp://localhost:10"])
                .Build())
            .WithEntityServices()
            .WithOpcUaServer()
            .WithOpcUaClientServices();
    }
}