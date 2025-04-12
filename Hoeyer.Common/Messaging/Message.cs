using System;

namespace Hoeyer.Common.Messaging;

public sealed record Message<T>(T Payload)
{
    public readonly Guid MessageId = Guid.NewGuid();
    public readonly T Payload = Payload;
    public readonly DateTime Timestamp = DateTime.UtcNow;
}