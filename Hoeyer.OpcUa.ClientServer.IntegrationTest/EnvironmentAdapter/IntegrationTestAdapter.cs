using System.Collections.Concurrent;
using Hoeyer.OpcUa.IntegrationTest.Extensions;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public static class IntegrationTestAdapter
{
    private static readonly ConcurrentDictionary<string, IIntegrationTestEnvironmentAdapter> CreatedAdapters = new();
    private static IIntegrationTestEnvironmentAdapterFactory? AdapterFactory { get; set; }

    public static IIntegrationTestEnvironmentAdapter GetSessionIsolatedAdapter() =>
        CreateOrGetCached(TestKeys.PerTestSessionKey);

    /// <summary>
    /// Checks cache and reuses adapter if one exists. Otherwise creates an adapter and adds it to the cache using <paramref name="cacheKey"/>
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    public static IIntegrationTestEnvironmentAdapter CreateOrGetCached(string cacheKey) =>
        CreatedAdapters.GetOrAdd(cacheKey,
            id => AdapterFactory?.GetIntegrationEnvironmentAdapter(id) ?? throw new NoFrameworkAdapterException());

    public static void AssignAdapter(IIntegrationTestEnvironmentAdapterFactory adapter) => AdapterFactory = adapter;

    public static void AssignFuncFactory(Func<string, IIntegrationTestEnvironmentAdapter> adapter)
    {
        AssignAdapter(new FuncBasedFactory(adapter));
    }

    public static void AssignFuncFactory(Func<string, IIntegrationTestEnvironment> adapter)
    {
        IIntegrationTestEnvironmentAdapter Factory(string s) => new SingletonAdapterFactory(adapter.Invoke(s));
        FuncBasedFactory factory = new FuncBasedFactory(Factory);
        AssignAdapter(factory);
    }


    private sealed class FuncBasedFactory(Func<string, IIntegrationTestEnvironmentAdapter> factory)
        : IIntegrationTestEnvironmentAdapterFactory
    {
        public IIntegrationTestEnvironmentAdapter GetIntegrationEnvironmentAdapter(string adapterId) =>
            factory.Invoke(adapterId);
    }

    private class SingletonAdapterFactory(IIntegrationTestEnvironment environment) : IIntegrationTestEnvironmentAdapter
    {
        public IIntegrationTestEnvironment TestEnvironment { get; } = environment;
    }
}