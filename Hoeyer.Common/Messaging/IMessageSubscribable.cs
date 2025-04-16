namespace Hoeyer.Common.Messaging;

public interface IMessageSubscribable<out T> 
{
    public ISubscription Subscribe(IMessageSubscriber<T> subscriber);
}