using Hoeyer.OpcUa.Client.Application.Browsing;
using Opc.Ua;

namespace Hoeyer.OpcUa.ClientTest.Generators;

public sealed class EntityBrowserTest
{
    [Test]
    [ClientServiceGenerator<IEntityBrowser>]
    [DisplayName("Can read node and children $services")]
    public async Task EntityBrowser_CanReadNodeAndChildren(ClientFixture<IEntityBrowser> services)
    {
        var fixture  = await services.GetFixture();
        var session = await services.CreateSession(Guid.NewGuid().ToString());
        var browseResult = await fixture.BrowseEntityNode(session, ObjectIds.RootFolder, CancellationToken.None);
        await Assert.That(browseResult.Node).IsNotNull();
        await Assert.That(browseResult.Children).IsNotEmpty();
    }
    
}