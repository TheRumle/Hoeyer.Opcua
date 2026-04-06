using Hoeyer.OpcUa.Test.Adapter;

namespace Hoeyer.OpcUa.Test.Simulation.Scope;

internal sealed class SimulationTestScope
{
    private readonly ITestFrameworkAdapter _adapter;
    private readonly string? _key;
    private readonly SharedType _sharedType;

    private SimulationTestScope(ITestFrameworkAdapter adapter, SharedType sharedType, string? key = null)
    {
        _adapter = adapter;
        _sharedType = sharedType;
        _key = key;
    }

    public T Create<T>(DataGeneratorMetadata metadata, Func<SimulationSetup, T> factory)
    {
        return SharedDataSources.GetOrCreate(
            _sharedType,
            metadata,
            _key,
            () => factory.Invoke(new SimulationSetup(_adapter)));
    }

    public static SimulationTestScope Keyed(ITestFrameworkAdapter adapter, string key) =>
        new(adapter, SharedType.Keyed, key);

    public static SimulationTestScope PerTestSession(ITestFrameworkAdapter adapter) =>
        new(adapter, SharedType.PerTestSession);

    public static SimulationTestScope PerClass(ITestFrameworkAdapter adapter) => new(adapter, SharedType.PerClass);
    public static SimulationTestScope PerTest(ITestFrameworkAdapter adapter) => new(adapter, SharedType.None);
}