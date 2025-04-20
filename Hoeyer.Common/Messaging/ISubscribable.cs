namespace Hoeyer.Common.Messaging;

public interface ISubscribable<out TMessage>
{
    public MessageSubscription Subscribe(IMessageConsumer<TMessage> subscriber);
}