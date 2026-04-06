using Hoeyer.OpcUa.Test.Adapter;

namespace Hoeyer.OpcUa.TestFramework.Test;

public static class PreDiscoverySetup
{
    [Before(TestDiscovery)]
    public static void AssignInstance()
    {
        FrameworkAdapter.Assign(new TestAdapter());
    }
}