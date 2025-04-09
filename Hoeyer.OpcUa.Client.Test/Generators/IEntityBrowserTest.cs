using Hoeyer.OpcUa.Client.Application.Browsing;
using Opc.Ua;

namespace Hoeyer.OpcUa.ClientTest.Generators;

[EntityBrowserGenerator]
public sealed class IEntityBrowserTest(OpcUaEntityBackendFixture<IEntityBrowser> browser)
{
    [Test]
    public async Task CanDoStuff()
    {
        var a  = await browser.GetFixture();
        var sessieon = await browser.GetSession(Guid.NewGuid().ToString());
        var data = await a.BrowseEntityNode(sessieon, ObjectIds.RootFolder, CancellationToken.None);
        await Assert.That(data.Node).IsNotNull();
        await Assert.That(data.Children).IsNotEmpty();
    }
    
}