using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.IntegrationTest.Fixture;
using Hoeyer.OpcUa.TestEntities;
using JetBrains.Annotations;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.IntegrationTest.Application;

[TestSubject(typeof(AgentManager<>))]
[NotInParallel]
public sealed class AgentManagerTest
{
    [Test]
    [ServiceCollectionDataSource]
    public async Task GantryManager_When_BrowsingFor_AList_PropertyIsStatusGood(
        IStartableAgentServer startableServer,
        MaybeInitializedAgentManager<Gantry> initializedAgentManagers)
    {
        using IDisposable asserts = Assert.Multiple();
        await startableServer.StartAsync();

        var manager = initializedAgentManagers.Manager!;
        OperationContext context = CreateOperationContext(RequestType.Read);

        var nodeToRead = manager.ManagedAgent.Select(node => node.PropertyByBrowseName[nameof(Gantry.AList)]);
        var value = new ReadValueId
        {
            NodeId = nodeToRead.NodeId,
            AttributeId = Attributes.Value
        };

        var resultCollection = new List<DataValue>
        {
            new()
        };
        var statuses = new List<ServiceResult>
        {
            new(ServiceResult.Good)
        };

        manager.Read(context, double.MaxValue - 123, [value], resultCollection, statuses);
        DataValue result = resultCollection[0];

        await Assert.That(statuses.Where(e => StatusCode.IsBad(e.StatusCode)))
            .IsEmpty()
            .Because(
                $" there should not be any errors when reading the list, but it was '{statuses[0].ToLongString()}'");

        await Assert.That(result.Value).IsNotNull().Because(" the read value should not be null");
    }

    private static OperationContext CreateOperationContext(RequestType type) =>
        new(new RequestHeader
        {
            RequestHandle = Random.Shared.GetUInt(),
            Timestamp = DateTime.Now
        }, type);

    [Test]
    [ServiceCollectionDataSource]
    [DisplayName("When server is started all properties have good statuscode")]
    public async Task WhenServerIsStarted_NoNodeHasNotGoodStatus(
        IStartableAgentServer startableServer,
        List<IMaybeInitializedAgentManager> maybeInitializedAgentManagers)
    {
        await startableServer.StartAsync();

        if (!maybeInitializedAgentManagers.Any()) Assert.Fail("No managers were initialized.");

        using IDisposable a = Assert.Multiple();
        foreach (IMaybeInitializedAgentManager maybeInitializedManager in maybeInitializedAgentManagers)
        {
            if (maybeInitializedManager.Manager == null)
            {
                Assert.Fail($"The manager for Agent '{maybeInitializedManager.AgentName}' was null");
                continue;
            }

            await AssertNoBadStatusOrNull(maybeInitializedManager);
        }
    }

    private static async Task AssertNoBadStatusOrNull(IMaybeInitializedAgentManager maybeInitializedManager)
    {
        using IDisposable assertScope = Assert.Multiple();
        var badProperties =
            maybeInitializedManager.Manager!.ManagedAgent.Select(node => node.PropertyByBrowseName);

        foreach ((var name, PropertyState value) in badProperties)
        {
            if (StatusCode.IsNotGood(value.StatusCode))
            {
                Assert.Fail($"{maybeInitializedManager.AgentName}.{name} should not have a bad status code");
                continue;
            }

            await Assert.That(value.Value).IsNotNull().Because($" the value of property {name} should never be null");
        }
    }
}