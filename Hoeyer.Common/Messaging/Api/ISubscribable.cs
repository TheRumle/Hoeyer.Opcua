namespace Hoeyer.Common.Messaging.Api;

public interface ISubscribable<TMessage>
{
    IMessageSubscription<TMessage> Subscribe(IMessageConsumer<TMessage> subscriber);
}