using System;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging;

public sealed record Message<T>(T Payload) : IMessage<T>
{
    public readonly Guid MessageId = Guid.NewGuid();
    public T Payload { get; } = Payload;
    public readonly DateTime Timestamp = DateTime.UtcNow;
}