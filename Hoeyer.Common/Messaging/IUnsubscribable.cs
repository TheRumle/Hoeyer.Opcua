namespace Hoeyer.Common.Messaging;

public interface IUnsubscribable
{
    void Unsubscribe(IMessageSubscription messageSubscription);
}