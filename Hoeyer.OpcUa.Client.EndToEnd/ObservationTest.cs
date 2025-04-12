using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Test.Client.EndToEnd.Generators;
using Hoeyer.OpcUa.TestApplication;

namespace Hoeyer.OpcUa.Test.Client.EndToEnd;

public sealed class ObservationTest
{
    private readonly ApplicationFixture _fixture = new();
    
    private sealed class TestSubscriber : IMessageSubscriber<Gantry>
    {
        public Gantry? Value { get; set; } 
        public void OnMessagePublished(IMessage<Gantry> message) => Value = (message.Payload);
    }

    
    [Test]
    public async Task WhenClientWritesToEntity_ObserverShouldBeNotified()
    {
        var session = await _fixture.CreateSession(Guid.NewGuid().ToString());
        var publisher = await _fixture.GetService<IMessageSubscribable<Gantry>>();
        var observer = new TestSubscriber();
        _ = publisher!.Subscribe(observer);

        var writer = await _fixture.GetService<IEntityWriter<Gantry>>();
        await writer!.AssignEntityValues(session, new Gantry
        {
            AList = ["helo"],
            IntValue = 678392,
            StringValue = "good strings my dear sir"
        });

        await Assert.That(observer.Value).IsNotNull();
    }
    
}