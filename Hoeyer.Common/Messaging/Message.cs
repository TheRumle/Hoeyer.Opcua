using System;

namespace Hoeyer.Common.Messaging;

public interface IMessage<out T>
{
    T Payload { get; }
}

public sealed record Message<T>(T Payload) : IMessage<T>
{
    public readonly Guid MessageId = Guid.NewGuid();
    public T Payload { get; } = Payload;
    public readonly DateTime Timestamp = DateTime.UtcNow;
}