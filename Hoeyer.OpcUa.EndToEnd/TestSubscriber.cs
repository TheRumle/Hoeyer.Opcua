using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.EndToEndTest.TestApplication;

namespace Hoeyer.OpcUa.EndToEndTest;

internal sealed class TestSubscriber<T> : IMessageSubscriber<T>
{
    public T Value { get; private set; } = default!; 
    public void OnMessagePublished(IMessage<T> message) => Value = (message.Payload);
}