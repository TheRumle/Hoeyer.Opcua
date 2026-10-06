using Hoeyer.OpcUa.Fixtures.Common.Test;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest;

[InheritsTests]
public sealed class FullSimulationResolutionTest() : DependencyResolutionTest(
    ServiceInjectionAttribute.Services.ServiceProvider.GetRequiredService<IServiceCollection>(), typeof(TestEntity));