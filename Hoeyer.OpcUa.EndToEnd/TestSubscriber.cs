using Hoeyer.Common.Messaging;

namespace Hoeyer.OpcUa.EndToEndTest;

internal sealed class TestSubscriber<T> : IMessageConsumer<T>
{
    public int Count { get; private set; }
    public void Consume(IMessage<T> message) => Count += 1;
}