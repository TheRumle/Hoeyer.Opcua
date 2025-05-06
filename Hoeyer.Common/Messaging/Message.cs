using System;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging;

public sealed record Message<T>(T Payload) : IMessage<T>
{
    public Guid MessageId { get; } = Guid.NewGuid();
    public T Payload { get; } = Payload;
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}