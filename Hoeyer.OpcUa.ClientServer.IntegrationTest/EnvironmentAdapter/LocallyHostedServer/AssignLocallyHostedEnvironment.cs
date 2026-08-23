namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter.LocallyHostedServer;

public static class AssignLocallyHostedEnvironment
{
    public const int LOCALHOST_DISCOVERY_ORDER = 1;

    [Before(TestDiscovery, Order = LOCALHOST_DISCOVERY_ORDER)]
    public static void AssignInstance()
    {
        IntegrationTestAdapter.AssignFuncFactory(s => new LocalHostedIntegrationTestEnvironment());
    }
}