namespace Hoeyer.Common.Messaging.Api;

public interface ISubscribable<out TMessage, out TOut> where TOut : IMessageSubscription<TMessage>
{
    TOut Subscribe(IMessageConsumer<TMessage> subscriber);
}

public interface ISubscribable<TMessage>
{
    IMessageSubscription<TMessage> Subscribe(IMessageConsumer<TMessage> subscriber);
}