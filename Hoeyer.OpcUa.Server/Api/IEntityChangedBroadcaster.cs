using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityChangedBroadcaster<T>
{
    ISubscriptionManager<T> EntitySubscriptionManager { get; }
    ISubscriptionManager<IEntityNode> NodeSubscriptionManager { get; }
    void BeginObserve(IEntityNode entityNode);
}