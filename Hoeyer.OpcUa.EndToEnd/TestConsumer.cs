using Hoeyer.Common.Messaging;

namespace Hoeyer.OpcUa.EndToEndTest;

internal sealed class TestConsumer<T> : IMessageConsumer<T>
{
    public int NumberOfMessages { get; private set; }
    public T Value { get; private set; } = default!; 
    public void Consume(IMessage<T> message)
    {
        NumberOfMessages += 1;
        Value = (message.Payload);
    }
}