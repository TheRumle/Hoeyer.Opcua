using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core.Extensions;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.TestApplication;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Hoeyer.OpcUa.Server.Entity;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.EndToEndTest;

[ClassDataSource<ApplicationFixture>]
public sealed class OpcUaEntityServerObservation(ApplicationFixture fixture)
{


    
    [Test]
    public async Task WhenClientWritesToEntity_ObserverShouldBeNotified()
    {
        var session = await fixture.CreateSession(Guid.NewGuid().ToString());
        var publisher = await fixture.GetService<IEntityChangedMessenger<Gantry>>();
        var observer = new TestSubscriber<Gantry>();
        _ = publisher!.Subscribe(observer);
        
        await WriteNode(session);
        await Assert.That(observer.Value).IsNotNull();
        await Assert.That(observer.Value.IntValue).IsEqualTo(2);
    }

    private async Task WriteNode(ISession session)
    {
        var reader = await fixture.GetService<IEntityBrowser<Gantry>>()!;
        var node = await reader.BrowseEntityNode(session, fixture.Token);
        var childToWrite = node.Children.First(e => e.BrowseName.Name.Equals(nameof(Gantry.IntValue)));

        await session.WriteAsync(null, new WriteValueCollection
        {
            new WriteValue()
            {
                NodeId = childToWrite.NodeId.AsNodeId(session.NamespaceUris),
                AttributeId = Attributes.Value,
                Value = new DataValue()
                {
                    Value = 2
                }
            }
        }, fixture.Token);
    }
}