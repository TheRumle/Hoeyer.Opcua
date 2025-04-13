using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Entity.Observability;

public interface IEntityChangedMessenger<T> : IMessagePublisher<T>
{
    void Publish(IEntityNode node);
}
public sealed class EntityChangedMessenger<T>(IMessagePublisher<T> publisher, IEntityTranslator<T> translator) :  IEntityChangedMessenger<T>
{
    public Subscription<T> Subscribe(IMessageSubscriber<T> subscriber) => publisher.Subscribe(subscriber);
    public void Publish(T message) => publisher.Publish(message);
    public void Publish(IEntityNode node) => Publish(translator.Translate(node));

    public void Unsubscribe(Subscription<T> subscription) => publisher.Unsubscribe(subscription);


}