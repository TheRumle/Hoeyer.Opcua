using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core.Proxy;
using Hoeyer.OpcUa.Test.Client.EndToEnd.Generators;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Test.Client.EndToEnd;

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