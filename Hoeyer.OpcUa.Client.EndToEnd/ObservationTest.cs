using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.EndToEnd.Generators;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.TestApplication;

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

        var message = new Gantry
        {
            AList = ["helo"],
            IntValue = 678392,
            StringValue = "good strings my dear sir"
        };
        var writer = fixture.GetService<IEntityWriter<Gantry>>();
        await writer!.AssignEntityValues(session, message);

        await Assert.That(observer.Value).IsNotNull();
        await Assert.That(observer.Value.IntValue).IsEqualTo(message.IntValue);
        await Assert.That(observer.Value.AList).IsEquivalentTo(message.AList);
        await Assert.That(observer.Value.StringValue).IsEquivalentTo(message.StringValue);
    }
    
}