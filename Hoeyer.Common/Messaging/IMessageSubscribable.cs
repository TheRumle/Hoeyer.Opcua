namespace Hoeyer.Common.Messaging;

public interface IMessageSubscribable<out T> 
{
    public IMessageSubscription Subscribe(IMessageSubscriber<T> subscriber);
}