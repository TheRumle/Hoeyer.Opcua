namespace Hoeyer.Common.Messaging.Api;

public interface IMessage<out T>
{
    T Payload { get; }
}