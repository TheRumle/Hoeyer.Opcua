namespace Hoeyer.Common.Messaging;

public interface ISubscribable<out TMessage>
{
    public ISubscription Subscribe(IMessageSubscriber<TMessage> subscriber);
}