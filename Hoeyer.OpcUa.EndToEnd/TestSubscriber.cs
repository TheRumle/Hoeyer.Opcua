using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.EndToEndTest;

internal sealed class TestSubscriber<T> : IMessageConsumer<T>
{
    public int Count { get; private set; } = 0;
    public void Consume(IMessage<T> changedProperties) => Count += 1;
}