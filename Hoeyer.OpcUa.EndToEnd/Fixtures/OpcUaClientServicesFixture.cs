using Hoeyer.OpcUa.Client.Services;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class OpcUaClientServicesFixture : OpcUaCoreServicesFixture
{
    public OpcUaClientServicesFixture()
    {
        OnGoingOpcEntityServiceRegistration.WithOpcUaClientServices();
    }
}