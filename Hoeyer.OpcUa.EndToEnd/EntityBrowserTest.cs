using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.EndToEndTest.Generators;

namespace Hoeyer.OpcUa.EndToEndTest;

public sealed class EntityBrowserTest
{
    [Test]
    [SingleServiceApplicationTestGenerator<IEntityBrowser>(typeof(IEntityBrowser<>))]
    public async Task EntityBrowser_CanReadNodeAndChildren(SingleServiceTestFixture<IEntityBrowser> services)
    {
        var fixture  = await services.GetClassUnderTest();
        var session =  await (await services.GetService<IEntitySessionFactory>()).CreateSessionAsync("Test");
        var browseResult = await fixture.BrowseEntityNode(session, CancellationToken.None);
        await Assert.That(browseResult.Node).IsNotNull();
        await Assert.That(browseResult.Children).IsNotEmpty();
    }
    
}