namespace Hoeyer.Common.Messaging;

public interface ISubscribable<out TMessage>
{
    public Subscription Subscribe(IMessageSubscriber<TMessage> subscriber);
}