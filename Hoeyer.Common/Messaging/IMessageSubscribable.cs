namespace Hoeyer.Common.Messaging;

public interface IMessageSubscribable<T>
{
    public Subscription<T> Subscribe(IMessageSubscriber<T> stateChangeSubscriber);
}