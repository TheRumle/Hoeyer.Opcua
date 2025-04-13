namespace Hoeyer.Common.Messaging;

public interface IMessage<out T>
{
    T Payload { get; }
}