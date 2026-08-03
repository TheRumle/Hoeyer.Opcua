using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;

namespace Hoeyer.OpcUa.IntegrationTest;

public static class TestCategories
{
    /// <summary>
    /// A category that requires an <see cref="IIntegrationTestEnvironmentAdapterFactory"/> to be defined in the assembly for the test to execute.
    /// <seealso cref="IntegrationTestFixture"/>
    /// <seealso cref="IntegrationTestFixture{T}"/>
    /// </summary>
    public const string INTEGRATION = "IntegrationTest";
}