using System.Collections.Concurrent;
using Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;

namespace Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

public static class ServerFixtureAdapter
{
    private static readonly ConcurrentDictionary<string, IServerFixture> CreatedServers = new();
    private static Func<string, IServerFixture> AdapterFactory { get; set; }

    public static IServerFixture GetSessionSharedServerFixture() =>
        CreateOrGetCached(TestKeys.PerTestSessionKey);

    /// <summary>
    ///     Checks cache and reuses adapter if one exists. Otherwise creates an adapter and adds it to the cache using
    ///     <paramref name="cacheKey" />
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    public static IServerFixture CreateOrGetCached(string cacheKey) =>
        CreatedServers.GetOrAdd(cacheKey,
            id => AdapterFactory(id) ?? throw new NoFrameworkAdapterException());

    public static void AssignServerFixtureFactory(Func<string, IServerFixture>  adapter) => AdapterFactory = adapter;
}