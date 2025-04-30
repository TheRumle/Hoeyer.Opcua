namespace Hoeyer.Common.Messaging.Api;

public interface IMessageConsumer<in T>
{
    public void Consume(IMessage<T> changedProperties);
}