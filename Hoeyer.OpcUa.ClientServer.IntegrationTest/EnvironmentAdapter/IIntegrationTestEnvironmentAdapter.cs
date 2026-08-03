using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public interface IIntegrationTestEnvironmentAdapter
{
    Type[] ClientAssemblyMarkers { get; }
    Type[] EntityAssemblyMarkers { get; }
    IIntegrationTestEnvironment TestEnvironment { get; }
    IServiceCollection ApplicationServices { get; }
}