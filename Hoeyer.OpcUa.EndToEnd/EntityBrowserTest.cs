using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;

namespace Hoeyer.OpcUa.EndToEndTest;

public sealed class EntityBrowserTest
{   
    [Test]
    [ApplicationFixtureGenerator<IEntityBrowser>]
    public async Task EntityBrowser_CanCreateEntityNode_AndTranslateIt(ApplicationFixture<IEntityBrowser> services)
    {
        var entity = await services.ExecuteWithSessionAsync(
            (session, browser) => browser.BrowseEntityNode(CancellationToken.None));
        
        await Assert.That(entity).IsNotDefault();
        await Assert.That(entity.PropertyStates).IsNotEmpty();
        await Assert.That(entity.BaseObject).IsNotDefault();
    }
}