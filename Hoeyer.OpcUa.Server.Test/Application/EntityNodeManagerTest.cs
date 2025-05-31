using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.IntegrationTest.Fixture;
using Hoeyer.opcUa.TestEntities;
using JetBrains.Annotations;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.IntegrationTest.Application;

[TestSubject(typeof(EntityNodeManager<>))]
[NotInParallel]
public sealed class EntityNodeManagerTest
{
    [Test]
    [ServiceCollectionDataSource]
    public async Task GantryManager_When_BrowsingFor_AList_PropertyIsStatusGood(
        IStartableEntityServer startableServer,
        MaybeInitializedEntityManager<Gantry> initializedEntityManagers)
    {
        using IDisposable asserts = Assert.Multiple();
        await startableServer.StartAsync();

        IEntityNodeManager manager = initializedEntityManagers.Manager!;
        OperationContext context = CreateOperationContext(RequestType.Read);

        PropertyState nodeToRead = manager.ManagedEntity.PropertyByBrowseName[nameof(Gantry.AList)];
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
        IStartableEntityServer startableServer,
        List<IMaybeInitializedEntityManager> maybeInitializedEntityManagers)
    {
        await startableServer.StartAsync();

        if (!maybeInitializedEntityManagers.Any()) Assert.Fail("No managers were initialized.");

        using IDisposable a = Assert.Multiple();
        foreach (IMaybeInitializedEntityManager maybeInitializedManager in maybeInitializedEntityManagers)
        {
            if (maybeInitializedManager.Manager == null)
            {
                Assert.Fail($"The manager for Entity '{maybeInitializedManager.EntityName}' was null");
                continue;
            }

            await AssertNoBadStatusOrNull(maybeInitializedManager);
        }
    }

    private static async Task AssertNoBadStatusOrNull(IMaybeInitializedEntityManager maybeInitializedManager)
    {
        using IDisposable assertScope = Assert.Multiple();
        Dictionary<string, PropertyState> badProperties =
            maybeInitializedManager.Manager!.ManagedEntity.PropertyByBrowseName;
        foreach ((var name, PropertyState value) in badProperties)
        {
            if (StatusCode.IsNotGood(value.StatusCode))
            {
                Assert.Fail($"{maybeInitializedManager.EntityName}.{name} should not have a bad status code");
                continue;
            }

            await Assert.That(value.Value).IsNotNull().Because($" the value of property {name} should never be null");
        }
    }
}