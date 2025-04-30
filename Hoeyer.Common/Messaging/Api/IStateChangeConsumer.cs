using System.Collections.Generic;

namespace Hoeyer.Common.Messaging.Api;

public interface IStateChangeConsumer<TSubject, TValue> : IMessageConsumer<Dictionary<TSubject, StateChange<TValue>>>;