using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityChangedBroadcaster
{
    void BeginObserve(IEntityNode entityNode);
    ISubscriptionManager<IEntityNode> NodeSubscriptionManager { get; } 
}

public interface IEntityChangedBroadcaster<T> : IEntityChangedBroadcaster
{
    ISubscriptionManager<T> EntitySubscriptionManager { get; } 
}