using Hoeyer.OpcUa.IntegrationTest.Extensions;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Hoeyer.OpcUa.Server;
using Hoeyer.OpcUa.Server.Application;

namespace Hoeyer.OpcUa.IntegrationTest.Configuration;

[DependsOn<HealthyOpcUaServer>]
[IntegrationServiceInjection]
public class ManagerHolderTests(
    IEnumerable<IEntityManagerHolder> managerHolders)
{
    public IEnumerable<TestDataRow<IEntityManagerHolder>> GetManagers()
    {
        return managerHolders.Select(node => new TestDataRow<IEntityManagerHolder>(
            node,
            DisplayName:
            $"When healthcheck is done, the entity node for '{node.EntityName}' is available"
        ));
    }

    [Test]
    [DependsOn<HealthyOpcUaServer>]
    [Category("Managers are healthy")]
    [InstanceMethodDataSource(nameof(GetManagers))]
    public async Task NodesAreHealthy(IEntityManagerHolder holder, CancellationToken timeout)
    {
        await Assert.That(holder).IsNotNull();
        await Assert.That(holder.HasValue).IsTrue();
    }


    [Test]
    [DisplayName("The server is holding the same managerholders as the ones that are injected")]
    [IntegrationServiceInjection]
    public async Task ServerHoldsSpecificManagerHolder(
        IEntityManagerHolder<TestEntity> managerHolder,
        IOpcEntityServer server,
        CancellationToken timeout)
    {
        await Assert.That(managerHolder as IEntityManagerHolder).IsContainedIn(server.Managers);
    }


    [Test]
    [DisplayName("The server is holding a reference to a specific manager")]
    [IntegrationServiceInjection]
    public async Task HoldsCorrectReferences(
        IEntityManagerHolder<TestEntity> managerHolder,
        CancellationToken timeout)
    {
        await Assert.That(managerHolder as IEntityManagerHolder).IsContainedIn(managerHolders);
    }

    [Test]
    [DisplayName($"A specific {nameof(IEntityManagerHolder)} can be provided")]
    [IntegrationServiceInjection]
    public async Task AManagerHolderCanBeProvided(
        IEntityManagerHolder<TestEntity> managerHolder,
        CancellationToken timeout)
    {
        await Assert.That(managerHolder).IsNotNull();
    }

    [Test]
    [DisplayName($"A {nameof(IOpcEntityServer)} can be provided")]
    [IntegrationServiceInjection]
    public async Task AServerCanBeProvided(
        IOpcEntityServer server,
        CancellationToken timeout)
    {
        await Assert.That(server).IsNotNull();
    }
}