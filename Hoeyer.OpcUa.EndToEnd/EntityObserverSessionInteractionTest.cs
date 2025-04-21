using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Application.Monitoring;
using Hoeyer.OpcUa.Core.Extensions;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.TestApplication;
using Opc.Ua;

namespace Hoeyer.OpcUa.EndToEndTest;

[ClassDataSource<ApplicationFixture>]
public class EntityObserverSessionInteractionTest(ApplicationFixture fixture)
{
    [Test]
    public async Task WhenSessionIsClosed_SubscriptionsArePreserved()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = await fixture.GetService<EntitySubscription<Gantry>>();
        _ = await monitor.SubscribeToChange(observer);
        
        await monitor.Session!.CloseAsync();
        await monitor.Session.ReconnectAsync();
        
        await WriteNode();
        await Assert.That(observer.Count).IsEqualTo(1);
    }
    
    private async Task WriteNode()
    {
        var session = await fixture.CreateSession(Guid.NewGuid().ToString());
        var reader = await fixture.GetService<IEntityBrowser<Gantry>>()!;
        var node = await reader.BrowseEntityNode(session, fixture.Token);
        var childToWrite = node.Children.First(e => e.BrowseName.Name.Equals(nameof(Gantry.IntValue)));

        await session.WriteAsync(null, new WriteValueCollection
        {
            new WriteValue
            {
                NodeId = childToWrite.NodeId.AsNodeId(session.NamespaceUris),
                AttributeId = Attributes.Value,
                Value = new DataValue
                {
                    Value = 2
                }
            }
        }, fixture.Token);
    }


    
}