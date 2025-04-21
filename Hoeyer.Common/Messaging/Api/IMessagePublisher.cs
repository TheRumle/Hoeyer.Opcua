namespace Hoeyer.Common.Messaging.Api;

public interface IMessagePublisher<in T> 
{
    public void Publish(T message);
}