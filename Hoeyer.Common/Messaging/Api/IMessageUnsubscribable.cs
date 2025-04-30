namespace Hoeyer.Common.Messaging.Api;

public interface IMessageUnsubscribable
{
    void Unsubscribe(IMessageSubscription messageSubscription);
}