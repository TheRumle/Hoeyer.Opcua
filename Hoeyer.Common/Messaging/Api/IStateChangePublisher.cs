using System.Collections.Generic;

namespace Hoeyer.Common.Messaging.Api;

public interface IStateChangePublisher<TSubject, TValue> : IMessagePublisher<IEnumerable<StateChange<TSubject, TValue>>>;
public interface IStateChangeConsumer<TSubject, TValue> : IMessageConsumer<IEnumerable<StateChange<TSubject, TValue>>>;
