using System.Reflection;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.OpcUa.EndToEndTest.TestEntities;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(IEntityBrowser<>))]
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
        var nullValues = entity.PropertyStates.Where(e => e.Value is null);
        await Assert.That(nullValues).IsEmpty().Because(" after reading the node should never have null values");
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
            .Select( property => (property.Name, Value:  property.GetValue(gantry)))
            .Where( e => e.Value == null);
        
        await Assert
            .That(propsWithNullValue)
            .IsEmpty()
            .Because( " after reading the node and translating it, there should not be null values");
    }
}