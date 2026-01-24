using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Test;
using Hoeyer.OpcUa.Test.Adapter.Client.Api;
using Hoeyer.OpcUa.Test.Simulation;
using JetBrains.Annotations;
using Opc.Ua;

namespace OpcUa.Client.TestFramework;

[TestSubject(typeof(IEntityBrowser<>))]
[TestSubject(typeof(INodeReader))]
[ClassDataSource<AdaptedSharedSimulationServiceContext<IEntityBrowser>>
    (Key = FixtureKeys.ReadOnlyFixture, Shared = SharedType.Keyed)]
[DependsOn<AdapterTest>]
public class EntityBrowserTest(List<ISpecifiedTestSession<IEntityBrowser>> simulationFixtures)
{
    [Test]
    public async Task EntityBrowser_CanCreateEntityNode_AndTranslateIt(
        CancellationToken token)
    {
        async Task CanGetAnyValuesFromEntity(IEntityBrowser browser)
        {
            var entity = await browser.BrowseEntityNode(token);
            await Assert.That(entity).IsNotNull();
            await Assert.That(entity.PropertyStates).IsNotEmpty();
            await Assert.That(entity.BaseObject).IsNotNull();
        }

        await simulationFixtures.AssertThatService(CanGetAnyValuesFromEntity);
    }

    [Test]
    public async Task EntityBrowser_BrowsedEntity_DoesNotHaveNullValues(CancellationToken token)
    {
        await simulationFixtures.AssertThatService(async browser =>
        {
            var entity = await browser.BrowseEntityNode(token);
            var propertyValue = GetNullPropertyValues(entity);
            using (Assert.Multiple())
            {
                foreach (var key in propertyValue.Keys)
                {
                    Assert.Fail(
                        $"{entity.BaseObject.BrowseName.Name}.{key} was null and no browsed property should be null");
                }
            }
        });
    }


    [Test]
    public async Task CanBrowseOnSimultaneousThreads(CancellationToken token)
    {
        await simulationFixtures.AssertThatService(async browser =>
        {
            var first = new Thread(() => browser.BrowseEntityNode(token).GetAwaiter().GetResult());
            var second = new Thread(() => browser.BrowseEntityNode(token).GetAwaiter().GetResult());
            first.Start();
            second.Start();
            first.Join();
            second.Join();
        });
    }

    private static Dictionary<string, object?> GetNullPropertyValues(IEntityNode entity)
    {
        return entity.PropertyStates
            .Where(e => e.Value is null or Variant { Value: null })
            .ToDictionary<PropertyState, string, object>(prop => prop.BrowseName.Name, prop => null!)!;
    }
}