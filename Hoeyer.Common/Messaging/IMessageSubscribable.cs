namespace Hoeyer.Common.Messaging;

public interface IMessageSubscribable<out T> 
{
    MessageSubscription Subscribe(IMessageConsumer<T> subscriber);
}