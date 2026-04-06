using Hoeyer.OpcUa.Test.Adapter;
using Hoeyer.OpcUa.Test.Simulation.Scope;

namespace Hoeyer.OpcUa.Test.Api.Attributes;

public sealed class PerClassSimulationFixtureAttribute : DataSourceGeneratorAttribute<SimulationTestSession>
{
    protected override IEnumerable<Func<SimulationTestSession>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return
        [
            () => SimulationTestScope.PerClass(FrameworkAdapter.GetAdapterInstance())
                .Create(dataGeneratorMetadata, setup => new SimulationTestSession(setup))
        ];
    }
}