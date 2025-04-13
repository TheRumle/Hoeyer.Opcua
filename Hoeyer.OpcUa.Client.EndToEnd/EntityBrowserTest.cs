using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.EndToEnd.Generators;

namespace Hoeyer.OpcUa.Client.EndToEnd;

public sealed class EntityBrowserTest
{
    [Test]
    [ApplicationFixtureGenerator<IEntityBrowser>]
    [DisplayName("Can read node and children $services")]
    public async Task EntityBrowser_CanReadNodeAndChildren(ApplicationFixture<IEntityBrowser> services)
    {
        var fixture  = await services.GetFixture();
        var session = await services.CreateSession(Guid.NewGuid().ToString());
        var browseResult = await fixture.BrowseEntityNode(session, CancellationToken.None);
        await Assert.That(browseResult.Node).IsNotNull();
        await Assert.That(browseResult.Children).IsNotEmpty();
    }
    
}