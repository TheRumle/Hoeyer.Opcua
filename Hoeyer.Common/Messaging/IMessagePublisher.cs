namespace Hoeyer.Common.Messaging;

public interface IMessagePublisher<T> : IMessageSubscribable<T>
{
    public void Publish(T message);
}