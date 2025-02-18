using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions.Exceptions;
using Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;
using Hoeyer.OpcUa.Server.Application.Node.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity;

public sealed class EntityNodeManager(
        ManagedEntityNode managedEntity,
        IServerInternal server,
        IEntityHandleManager handleManager,
        IEntityModifier entityModifier,
        IEntityBrowser browser,
        ILogger logger
    ) : INodeManager2
{
    public readonly ManagedEntityNode ManagedEntity = managedEntity;
    public IEnumerable<string> NamespaceUris { get; } = [managedEntity.Namespace];
    private readonly ServerSystemContext _systemContext = server.DefaultSystemContext;

    /// <inheritdoc />
    public void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
    }


    /// <inheritdoc />
    public void DeleteAddressSpace()
    {
    }

    /// <inheritdoc />
    public object? GetManagerHandle(NodeId nodeId) => handleManager.GetEntityHandle(nodeId);

    /// <inheritdoc />
    public void AddReferences(IDictionary<NodeId, IList<IReference>> references)
    {
        var exceptions = references
            .CreateEntityViolations(ManagedEntity, e => e.Key, e => new NoSuchManagedNodeException(ManagedEntity, e.Key))
            .ToList();
        if (exceptions.Any())
        {
            var exception = exceptions.ToAggregateException();
            logger.LogError(exception.Message);
            throw exception;
        }
        entityModifier.AddReferences(references);
    }

    /// <inheritdoc />
    public ServiceResult DeleteReference(
        object sourceHandle,
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId,
        bool deleteBidirectional) => entityModifier.DeleteReference(sourceHandle, referenceTypeId, isInverse, targetId);

    /// <inheritdoc />
    public NodeMetadata GetNodeMetadata(OperationContext context, object targetHandle, BrowseResultMask resultMask)
    {
        if (!handleManager.IsEntityHandle(targetHandle))
        {
            var e = new NoSuchManagedNodeException(ManagedEntity, targetHandle);
            logger.LogError(e.Message);
            throw e;
        }

        var serverContext = _systemContext.Copy();
        return ManagedEntity.ConstructMetadata(serverContext);
    }

    /// <inheritdoc />
    public void Browse(OperationContext context, ref ContinuationPoint continuationPoint,
        IList<ReferenceDescription> references)
    {
        var toTake = continuationPoint.MaxResultsToReturn - references.Count;
        if (toTake <= 0) return;
        
        foreach (var description  in browser.Browse(continuationPoint, _systemContext.Copy(context)).Take((int)toTake))
        {
            references.Add(description);
        }
    }

    /// <inheritdoc />
    public void Write(OperationContext context, IList<WriteValue> nodesToWrite, IList<ServiceResult> errors)
    {
        var exceptions = nodesToWrite.CreateEntityViolations(ManagedEntity, wv => wv.NodeId, wv => new NoSuchManagedNodeException(ManagedEntity, wv.NodeId)).ToList();

        if (exceptions.Count != 0)
        {
            var e = exceptions.ToAggregateException();
            logger.LogWarning(e, "Entity manager for {@Entity} was invoked with write operation to node(s) that was unrelated to the entity.", ManagedEntity);
            throw e;
        }
        var systemContext = _systemContext.Copy(context);
        entityModifier.Write(systemContext, nodesToWrite);
    }
    

    /// <inheritdoc />
    public void TranslateBrowsePath(OperationContext context, object sourceHandle, RelativePathElement relativePath,
        IList<ExpandedNodeId> targetIds, IList<NodeId> unresolvedTargetIds)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Read(OperationContext context, double maxAge, IList<ReadValueId> nodesToRead, IList<DataValue> values,
        IList<ServiceResult> errors)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void HistoryRead(OperationContext context, HistoryReadDetails details, TimestampsToReturn timestampsToReturn,
        bool releaseContinuationPoints, IList<HistoryReadValueId> nodesToRead, IList<HistoryReadResult> results,
        IList<ServiceResult> errors)
    {
        throw new NotImplementedException();
    }


    /// <inheritdoc />
    public void HistoryUpdate(OperationContext context, Type detailsType, IList<HistoryUpdateDetails> nodesToUpdate,
        IList<HistoryUpdateResult> results, IList<ServiceResult> errors)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Call(OperationContext context, IList<CallMethodRequest> methodsToCall, IList<CallMethodResult> results,
        IList<ServiceResult> errors)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ServiceResult SubscribeToEvents(OperationContext context, object sourceId, uint subscriptionId,
        IEventMonitoredItem monitoredItem, bool unsubscribe)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ServiceResult SubscribeToAllEvents(OperationContext context, uint subscriptionId,
        IEventMonitoredItem monitoredItem,
        bool unsubscribe)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ServiceResult ConditionRefresh(OperationContext context, IList<IEventMonitoredItem> monitoredItems)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void CreateMonitoredItems(OperationContext context, uint subscriptionId, double publishingInterval,
        TimestampsToReturn timestampsToReturn, IList<MonitoredItemCreateRequest> itemsToCreate,
        IList<ServiceResult> errors,
        IList<MonitoringFilterResult> filterErrors, IList<IMonitoredItem> monitoredItems,
        ref long globalIdCounter)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void ModifyMonitoredItems(OperationContext context, TimestampsToReturn timestampsToReturn,
        IList<IMonitoredItem> monitoredItems,
        IList<MonitoredItemModifyRequest> itemsToModify, IList<ServiceResult> errors,
        IList<MonitoringFilterResult> filterErrors)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void DeleteMonitoredItems(OperationContext context, IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void TransferMonitoredItems(OperationContext context, bool sendInitialValues,
        IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void SetMonitoringMode(OperationContext context, MonitoringMode monitoringMode,
        IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc />
    public void SessionClosing(OperationContext context, NodeId sessionId, bool deleteSubscriptions)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool IsNodeInView(OperationContext context, NodeId viewId, object nodeHandle)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public NodeMetadata GetPermissionMetadata(OperationContext context, object targetHandle,
        BrowseResultMask resultMask,
        Dictionary<NodeId, List<object>> uniqueNodesServiceAttributesCache, bool permissionsOnly)
    {
        throw new NotImplementedException();
    }

   
}