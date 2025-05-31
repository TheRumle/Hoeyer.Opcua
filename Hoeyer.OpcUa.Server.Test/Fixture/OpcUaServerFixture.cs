using Hoeyer.OpcUa.Core.Test.Fixtures;
using Hoeyer.OpcUa.Server.Services;

namespace Hoeyer.OpcUa.Server.IntegrationTest.Fixture;

public sealed class OpcUaServerServiceFixture : OpcUaCoreServicesFixture
{
    public OpcUaServerServiceFixture()
    {
        OnGoingOpcEntityServiceRegistration.WithOpcUaServer();
    }
}