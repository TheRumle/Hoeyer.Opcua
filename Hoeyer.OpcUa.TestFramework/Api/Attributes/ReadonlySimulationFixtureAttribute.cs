using Hoeyer.OpcUa.Test.Adapter;
using Hoeyer.OpcUa.Test.Simulation.Scope;

namespace Hoeyer.OpcUa.Test.Api.Attributes;

public sealed class ReadonlySimulationFixtureAttribute<T>
    : DataSourceGeneratorAttribute<SimulationServiceContext<T>>
{
    protected override IEnumerable<Func<SimulationServiceContext<T>>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return
        [
            () => SimulationTestScope.PerTestSession(FrameworkAdapter.GetAdapterInstance())
                .Create(dataGeneratorMetadata, setup => new SimulationServiceContext<T>(setup))
        ];
    }
}

public sealed class ReadonlySimulationFixtureAttribute : DataSourceGeneratorAttribute<SimulationTestSession>
{
    protected override IEnumerable<Func<SimulationTestSession>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return
        [
            () => SimulationTestScope.PerTestSession(FrameworkAdapter.GetAdapterInstance())
                .Create(dataGeneratorMetadata, setup => new SimulationTestSession(setup))
        ];
    }
}