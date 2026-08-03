using System.Collections.Concurrent;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public static class IntegrationTestAdapter
{
    private static readonly ConcurrentDictionary<string, IIntegrationTestEnvironmentAdapter> CreatedAdapters = new();

    private static IIntegrationTestEnvironmentAdapterFactory? AdapterFactory { get; set; }

    /// <summary>
    /// Checks cache and reuses adapter if one exists. Otherwise creates an adapter and adds it to the cache using <paramref name="cacheKey"/>
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    public static IIntegrationTestEnvironmentAdapter CreateOrGetCached(string cacheKey) =>
        CreatedAdapters.GetOrAdd(cacheKey, CreateAdapter);

    public static void Assign(IIntegrationTestEnvironmentAdapterFactory adapter) => AdapterFactory = adapter;

    public static void AssignFuncFactory(Func<string, IIntegrationTestEnvironmentAdapter> adapter)
    {
        Assign(new FuncBasedFactory(adapter));
    }

    private static IIntegrationTestEnvironmentAdapter CreateAdapter(string id) =>
        AdapterFactory?.GetIntegrationEnvironmentAdapter(id) ?? throw new NoFrameworkAdapterException();

    private sealed class FuncBasedFactory(Func<string, IIntegrationTestEnvironmentAdapter> factory)
        : IIntegrationTestEnvironmentAdapterFactory
    {
        public IIntegrationTestEnvironmentAdapter GetIntegrationEnvironmentAdapter(string adapterId) =>
            factory.Invoke(adapterId);
    }
}