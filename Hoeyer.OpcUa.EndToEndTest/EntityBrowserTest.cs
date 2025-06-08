using System.Reflection;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.opcUa.TestEntities;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(IEntityBrowser<>))]
[TestSubject(typeof(INodeReader))]
public sealed class EntityBrowserTest
{
    [Test]
    [ApplicationFixtureGenerator<IEntityBrowser>]
    public async Task EntityBrowser_CanCreateEntityNode_AndTranslateIt(ApplicationFixture<IEntityBrowser> services)
    {
        var entity = await services.ExecuteAsync(browser => browser.BrowseEntityNode(CancellationToken.None));

        await Assert.That(entity).IsNotDefault();
        await Assert.That(entity.PropertyStates).IsNotEmpty();
        await Assert.That(entity.BaseObject).IsNotDefault();
    }

    [Test]
    [ApplicationFixtureGenerator<IEntityBrowser>]
    public async Task EntityBrowser_BrowsedEntity_DoesNotHaveNullValues(ApplicationFixture<IEntityBrowser> services)
    {
        var entity = await services.ExecuteAsync(browser => browser.BrowseEntityNode(CancellationToken.None));
        Dictionary<string, object?> propertyValue = GetNullPropertyValues(entity);

        using (Assert.Multiple())
        {
            foreach (var key in propertyValue.Keys)
            {
                Assert.Fail(
                    $"{entity.BaseObject.BrowseName.Name}.{key} was null and no browsed property should be null");
            }
        }
    }

    [Test]
    [TestSubject(typeof(IEntityTranslator<Gantry>))]
    [ClassDataSource<ApplicationFixture>]
    public async Task EntityBrowser_BrowsedEntity_CanBeTranslated_DoesNotHaveNullValues(ApplicationFixture services)
    {
        var browser = services.GetService<IEntityBrowser<Gantry>>();
        var translator = services.GetService<IEntityTranslator<Gantry>>();
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

    [ApplicationFixtureGenerator<IEntityBrowser>]
    [Test]
    public void CanBrowseOnSimultaneousThreads(ApplicationFixture<IEntityBrowser> fixture)
    {
        IEntityBrowser browser = fixture.TestedService;
        var first = new Thread(() => browser.BrowseEntityNode(CancellationToken.None).GetAwaiter().GetResult());
        var second = new Thread(() => browser.BrowseEntityNode(CancellationToken.None).GetAwaiter().GetResult());
        first.Start();
        second.Start();
        first.Join();
        second.Join();
    }

    private static Dictionary<string, object?> GetNullPropertyValues(IEntityNode entity)
    {
        return entity.PropertyStates
            .Where(e => e.Value is null or Variant { Value: null })
            .ToDictionary<PropertyState, string, object>(prop => prop.BrowseName.Name, prop => null!)!;
    }
}