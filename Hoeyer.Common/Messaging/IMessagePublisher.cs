namespace Hoeyer.Common.Messaging;

public interface IMessagePublisher<in T> 
{
    public void Publish(T message);
}