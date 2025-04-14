namespace Hoeyer.Common.Messaging;

public interface IMessageSubscribable<out T> 
{
    public Subscription Subscribe(IMessageSubscriber<T> subscriber);
}