namespace Hoeyer.Common.Messaging;

public interface ISubscribable<TMessage>
{
    public Subscription<TMessage> Subscribe(IMessageSubscriber<TMessage> stateChangeSubscriber);
}