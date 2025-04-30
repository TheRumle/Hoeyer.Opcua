using System;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaEntityService(typeof(IEntityChangedBroadcaster<>), ServiceLifetime.Singleton)]
public sealed class EntityChangedBroadcaster<T>(IEntityTranslator<T> translator) : IEntityChangedBroadcaster<T>
{
    private IEntityNode? Node { get; set; }
    public ISubscriptionManager<IEntityNode> NodeSubscriptionManager { get; } =  new SubscriptionManager<IEntityNode>();
    public ISubscriptionManager<T> EntitySubscriptionManager { get; private set; } = new SubscriptionManager<T>();

    /// <inheritdoc />
    public void BeginObserve(IEntityNode entityNode)
    {
        Node = entityNode;
        foreach (var propertyState in Node.PropertyStates)
        {
            propertyState.OnWriteValue += OnPropertyWritten;
        }
    }
    
    private void Publish()
    {
        if (Node == null) return;
        var entity = translator.Translate(Node);
        EntitySubscriptionManager.Publish(entity);
        NodeSubscriptionManager.Publish(Node);
    }
    
    private ServiceResult OnPropertyWritten(ISystemContext context, NodeState node, NumericRange indexrange, QualifiedName dataencoding, ref object value, ref StatusCode statuscode, ref DateTime timestamp)
    {
        if(StatusCode.IsGood(statuscode)) Publish();
        return ServiceResult.Good;
    }
}


