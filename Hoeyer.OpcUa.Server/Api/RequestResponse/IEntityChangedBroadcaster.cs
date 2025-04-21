using System.Collections.Generic;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api.RequestResponse;

public interface IEntityChangedBroadcaster : ISubscribable<IEntityNode>,
    ISubscribable<IEnumerable<StateChange<PropertyState, object>>>,
    IMessagePublisher<(IEntityNode NewState, IEnumerable<StateChange<PropertyState, object>> Changes)>;

public interface IEntityChangedBroadcaster<T> : IEntityChangedBroadcaster 
{
    ISubscribable<T> EntitySubcribable { get; }
}