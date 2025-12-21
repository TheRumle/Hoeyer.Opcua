using System.Reflection;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.EndToEndTest.Extensions;
using Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;
using JetBrains.Annotations;
using Opc.Ua;
using Playground.Modelling.Models;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[TestSubject(typeof(IEntityBrowser<>))]
[TestSubject(typeof(INodeReader))]
[ClassDataSource<SimulationFixture<IEntityBrowser>>(Shared = SharedType.Keyed, Key = FixtureKeys.ReadOnlyFixture)]
public sealed class EntityBrowserTest(SimulationFixture<IEntityBrowser> simulationFixture)
{
    [Test]
    public async Task EntityBrowser_CanCreateEntityNode_AndTranslateIt(
        CancellationToken token)
    {
        await simulationFixture.AssertAll(async service =>
        {
            var entity = await service.BrowseEntityNode(token);
            await Assert.That(entity).IsNotNull();
            await Assert.That(entity.PropertyStates).IsNotEmpty();
            await Assert.That(entity.BaseObject).IsNotNull();
        });
    }

    [Test]
    public async Task EntityBrowser_BrowsedEntity_DoesNotHaveNullValues(CancellationToken token)
    {
        await simulationFixture.AssertInteractions(async serviceUnderTest =>
        {
            var entity = await serviceUnderTest.ExecuteAsync(browser => browser.BrowseEntityNode(token));
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
    [TestSubject(typeof(IEntityTranslator<Gantry>))]
    [TestSubject(typeof(IEntityBrowser<Gantry>))]
    public async Task EntityBrowser_BrowsedEntity_CanBeTranslated_DoesNotHaveNullValues()
    {
        var browser = simulationFixture.GetService<IEntityBrowser<Gantry>>();
        var translator = simulationFixture.GetService<IEntityTranslator<Gantry>>();
        var node = await browser.BrowseEntityNode();
        var gantry = translator.Translate(node);

        var propsWithNullValue = gantry
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => (property.Name, Value: property.GetValue(gantry)))
            .Where(e => e.Value == null);

        await Assert
            .That(propsWithNullValue)
            .IsEmpty()
            .Because(" after reading the node and translating it, there should not be null values");
    }

    [Test]
    public async Task CanBrowseOnSimultaneousThreads(CancellationToken token) =>
        await simulationFixture.AssertInteractions(serviceUnderTest =>
        {
            var browser = serviceUnderTest.TestedService;
            var first = new Thread(() => browser.BrowseEntityNode(token).GetAwaiter().GetResult());
            var second = new Thread(() => browser.BrowseEntityNode(token).GetAwaiter().GetResult());
            first.Start();
            second.Start();
            first.Join();
            second.Join();
        });

    private static Dictionary<string, object?> GetNullPropertyValues(IEntityNode entity)
    {
        return entity.PropertyStates
            .Where(e => e.Value is null or Variant { Value: null })
            .ToDictionary<PropertyState, string, object>(prop => prop.BrowseName.Name, prop => null!)!;
    }
}