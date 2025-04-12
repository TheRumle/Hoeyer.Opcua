namespace Hoeyer.Common.Messaging;

public interface IMessageSubscriber<in T>
{
    public void OnMessagePublished(IMessage<T> message);
}