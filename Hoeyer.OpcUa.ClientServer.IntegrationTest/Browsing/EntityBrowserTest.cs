using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Browsing.Reading;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.IntegrationTest.AbstractTests;

[TestSubject(typeof(IEntityBrowser<>))]
[TestSubject(typeof(INodeReader))]
[IntegrationAdapterDependentTest]
[DependsOn<IntegrationEnvironmentHealthTests>]
public abstract class EntityBrowserTest<T>(IntegrationTestFixture<IEntityBrowser<T>> context)
{
    [Test]
    [DisplayName("Can browse entity nodes and translate it to property states and base object")]
    public async Task EntityBrowser_CanCreateEntityNode_AndTranslateIt(
        CancellationToken token)
    {
        var entity = await context.ExecuteAsync(browser => browser.BrowseEntityNode(token));
        await Assert.That(entity).IsNotNull();
        await Assert.That(entity.PropertyStates).IsNotEmpty();
        await Assert.That(entity.BaseObject).IsNotNull();
    }

    [Test]
    [DisplayName("The browsed entity is not a null value")]
    public async Task EntityBrowser_BrowsedEntity_DoesNotHaveNullValues(CancellationToken token)
    {
        var entity = await context.ExecuteAsync(browser => browser.BrowseEntityNode(token));
        var propertyValue = GetNullPropertyValues(entity);
        foreach (var key in propertyValue.Keys)
        {
            Assert.Fail(
                $"{entity.BaseObject.BrowseName.Name}.{key} was null and no browsed property should be null");
        }
    }


    [Test]
    [DisplayName("The browser can browse on at least two virtual threads")]
    public async Task CanBrowseOnSimultaneousThreads(CancellationToken token)
    {
        var browse = async () =>
        {
            var entity = await context.ExecuteAsync(browser => browser.BrowseEntityNode(token));
            await Assert.That(entity).IsNotNull();
            await Assert.That(entity.PropertyStates).IsNotEmpty();
            await Assert.That(entity.BaseObject).IsNotNull();
        };

        var first = Task.Run(browse, token);
        var second = Task.Run(browse, token);
        await Task.WhenAll(first, second);
    }

    private static Dictionary<string, object?> GetNullPropertyValues(IEntityNode entity)
    {
        return entity.PropertyStates
            .Where(e => e.Value is null or Variant { Value: null })
            .ToDictionary<PropertyState, string, object>(prop => prop.BrowseName.Name, prop => null!)!;
    }
}