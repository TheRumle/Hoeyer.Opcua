using Hoeyer.OpcUa.Test.Adapter;

namespace Playground.Application.EndToEndTest;

public static class PreDiscoverySetup
{
    [Before(TestDiscovery)]
    public static void AssignInstance()
    {
        FrameworkAdapter.Assign(new EndToEndTestSimulationSetup());
    }
}