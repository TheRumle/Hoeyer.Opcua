namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public sealed class NoFrameworkAdapterException() : Exception(ErrorMessage)
{
    internal static readonly string ErrorMessage =
        $"No {nameof(IIntegrationTestEnvironmentAdapterFactory)} was assigned. " +
        $"Use {nameof(IntegrationTestAdapter)}.{nameof(IntegrationTestAdapter.Assign)} in a method annotated with " +
        $"{nameof(BeforeAttribute)}({TestDiscovery}) to assign an integration environment adapter";
}