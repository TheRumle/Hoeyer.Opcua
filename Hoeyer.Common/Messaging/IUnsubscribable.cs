namespace Hoeyer.Common.Messaging;

public interface IUnsubscribable
{
    public void Unsubscribe(IMessageSubscription messageSubscription);
}