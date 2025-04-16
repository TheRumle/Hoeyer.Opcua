namespace Hoeyer.Common.Messaging;

public interface ISubscribable<out TMessage>
{
    public IMessageSubscription Subscribe(IMessageSubscriber<TMessage> subscriber);
}