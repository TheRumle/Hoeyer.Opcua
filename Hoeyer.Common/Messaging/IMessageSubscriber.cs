namespace Hoeyer.Common.Messaging;

public interface IMessageConsumer<in T>
{
    public void Consume(IMessage<T> message);
}