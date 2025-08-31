using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Test.Fixtures;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class OpcUaClientServicesFixtureAttribute : OpcUaCoreServicesFixtureAttribute
{
    public OpcUaClientServicesFixtureAttribute()
    {
        OnGoingOpcEntityServiceRegistration.WithOpcUaClientServices();
    }
}