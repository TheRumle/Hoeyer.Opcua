using System;

namespace Hoeyer.Common.Messaging.Api;

public interface IMessage<out T>
{
    public Guid MessageId { get; }
    public T Payload { get; } 
    public DateTime Timestamp { get; }
}