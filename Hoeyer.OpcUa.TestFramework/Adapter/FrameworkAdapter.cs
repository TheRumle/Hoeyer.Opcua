namespace Hoeyer.OpcUa.Test.Adapter;

public static class FrameworkAdapter
{
    public static ITestFrameworkAdapter? AdapterInstance { get; private set; }

    public static ITestFrameworkAdapter GetAdapterInstance() =>
        AdapterInstance ?? throw new NoFrameworkAdapterException(
            $"No adapter instance configured. Did you remember to implement an {nameof(ITestFrameworkAdapter)} in the test assembly?");

    public static void Assign(ITestFrameworkAdapter adapter) => AdapterInstance = adapter;
}