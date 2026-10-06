namespace Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

public sealed class NoFrameworkAdapterException() : Exception(ErrorMessage)
{
    internal static readonly string ErrorMessage =
        $"No factory creating  {nameof(IServerFixture)} was assigned in  " +
        $"Use {nameof(ServerFixtureAdapter)}.{nameof(ServerFixtureAdapter.AssignServerFixtureFactory)} in a method annotated with " +
        $"{nameof(BeforeAttribute)}({TestDiscovery}) to assign an integration environment adapter";
}