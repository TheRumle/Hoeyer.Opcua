namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public static class DisposeCachedEnvironments
{
    [After(TestSession)]
    public static Task DisposeEnvironmentsAsync() =>
        IntegrationTestAdapter.DisposeCachedEnvironmentsAsync().AsTask();
}