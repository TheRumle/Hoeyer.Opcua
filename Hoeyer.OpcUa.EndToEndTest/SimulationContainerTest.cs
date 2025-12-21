using DotNet.Testcontainers.Containers;
using Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;

namespace Hoeyer.OpcUa.EndToEndTest;

public class SimulationContainerTest
{
    [ClassDataSource<OpcUaSimulationServerContainer>]
    public required OpcUaSimulationServerContainer Container { get; init; }

    [Test]
    public async Task CanConnectToOpcuaServer()
    {
        await Assert.That(Container.Container.Health)
            .IsEqualTo(TestcontainersHealthStatus.Healthy);
    }
}