using System.Collections.Generic;

namespace Hoeyer.Common.Messaging.Api;

public interface IStateChangePublisher<TSubject, TValue> : IMessagePublisher<Dictionary<TSubject, StateChange<TValue>>>;