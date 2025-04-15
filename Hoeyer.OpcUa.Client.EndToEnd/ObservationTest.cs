using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.EndToEnd.Generators;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Extensions;
using Hoeyer.OpcUa.Server.Entity;
using Hoeyer.OpcUa.TestApplication;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.EndToEnd;

[ClassDataSource<ApplicationFixture>]
public sealed class ObservationTest(ApplicationFixture fixture)
{
    private sealed class TestSubscriber : IMessageSubscriber<Gantry>
    {
        public Gantry Value { get; set; } = null!; 
        public void OnMessagePublished(IMessage<Gantry> message) => Value = (message.Payload);
    }

    
    [Test]
    public async Task WhenClientWritesToEntity_ObserverShouldBeNotified()
    {
        var session = await fixture.CreateSession(Guid.NewGuid().ToString());
        var publisher = fixture.GetService<IEntityChangedMessenger<Gantry>>();
        var observer = new TestSubscriber();
        _ = publisher!.Subscribe(observer);


        var reader = fixture.GetService<IEntityBrowser<Gantry>>()!;
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

        await Assert.That(observer.Value).IsNotNull();
        await Assert.That(observer.Value.IntValue).IsEqualTo(2);
    }
    
}