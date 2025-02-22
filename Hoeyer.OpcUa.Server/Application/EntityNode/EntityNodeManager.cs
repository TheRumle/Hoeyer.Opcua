using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.EntityNode.Operations;
using Hoeyer.OpcUa.Server.Application.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

internal sealed class EntityNodeManager(
    ManagedEntityNode managedEntity,
    IServerInternal server,
    IEntityHandleManager entityHandleManager,
    IEntityModifier entityModifier,
    IEntityBrowser browser,
    IEntityReader entityReader,
    IEntityReferenceManager referenceManager,
    ILogger logger
) : CustomNodeManager(server, managedEntity.Namespace), IEntityNodeManager
{
    private readonly BaseObjectState _entity = managedEntity.Entity;
    private readonly ServerSystemContext _systemContext = server.DefaultSystemContext;


    public IEntityNode ManagedEntity { get; } = managedEntity;

    /// <inheritdoc />
    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        logger.BeginScope("Creating address space and initializing {NodeName} nodes", managedEntity.Entity.BrowseName);
        try
        {
            var res = referenceManager.IntitializeNodeWithReferences(externalReferences);
            if (res.IsFailed) logger.LogError(res.Errors.ToNewlineSeparatedString());

            AddPredefinedNode(_systemContext, managedEntity.Entity);
            AddPredefinedNode(_systemContext, managedEntity.Folder);
            foreach (var properties in managedEntity.PropertyStates.Values)
            {
                logger.LogInformation("Adding {PropertyName}", properties.BrowseName);
                AddPredefinedNode(_systemContext, properties);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create address space.");
        }
    }


    /// <inheritdoc />
    public override void DeleteAddressSpace()
    {
        using var scope = logger.BeginScope("Disposing entity folder, entity and entity properties");
        ManagedEntity.Entity.Dispose();
        ManagedEntity.Folder.Dispose();
        foreach (var propertyStatesValue in ManagedEntity.PropertyStates.Values) propertyStatesValue.Dispose();
        ManagedEntity.PropertyStates.Clear();
    }

    /// <inheritdoc />
    public override object? GetManagerHandle(NodeId nodeId)
    {
        return entityHandleManager.GetHandle(nodeId).ValueOrDefault;
    }

    /// <inheritdoc />
    public override void AddReferences(IDictionary<NodeId, IList<IReference>> references)
    {
        using var scope = logger.BeginScope("Adding references {References}", references);

        foreach (KeyValuePair<NodeId, IList<IReference>> kvp in references.Where(e =>
                     entityHandleManager.GetState(e.Key).IsSuccess))
        {
            var result = AddReference(kvp);
            if (!result.IsSuccess)
                logger.LogWarning(
                    "Failed to references from {Node} --> {Targets}: {Error}",
                    kvp.Key,
                    kvp.Value.Select(e => e.ReferenceTypeId),
                    result.Errors.ToNewlineSeparatedString());
            else
                logger.LogInformation("Node {NodeId} now references targets: {References}", _entity.BrowseName,
                    kvp.Value.Select(e => e.TargetId));
        }
    }

    /// <inheritdoc />
    public override ServiceResult DeleteReference(
        object sourceHandle,
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId,
        bool deleteBidirectional)
    {
        using var scope = logger.BeginScope("Deleting references {Reference}", referenceTypeId);

        if (!_entity.ReferenceExists(referenceTypeId, isInverse, targetId))
        {
            logger.LogError("The {@Entity} does not reference node with reference type id '{@ReferenceTypeid}'",
                _entity.BrowseName, referenceTypeId);
            return StatusCodes.BadNodeIdUnknown;
        }

        var deletion = referenceManager.DeleteReference(referenceTypeId, isInverse, targetId);
        if (deletion.IsSuccess) return ServiceResult.Good;

        logger.LogError("Failed deleting reference {Reference}: {Errors}",
            referenceTypeId,
            deletion.Errors.ToNewlineSeparatedString());

        return StatusCodes.BadInvalidArgument;
    }

    /// <inheritdoc />
    public override NodeMetadata GetNodeMetadata(OperationContext context, object targetHandle,
        BrowseResultMask resultMask)
    {
        if (!entityHandleManager.IsHandleToAnyRelatedNode(targetHandle))
        {
            logger.LogError("The handle '{TargetHandle}' is not a handle related to entity {Entity}", targetHandle,
                _entity.BrowseName);
            return null!;
        }

        var serverContext = _systemContext.Copy();
        return ManagedEntity.ConstructMetadata(serverContext);
    }

    /// <inheritdoc />
    public override void Browse(OperationContext context, ref ContinuationPoint continuationPoint,
        IList<ReferenceDescription> references)
    {
        if (continuationPoint.NodeToBrowse is not NodeHandle nodeToBrowse)
        {
            logger.LogError(
                "Attempted to browse the manager, but the given node to browse was not a NodeHandle but was {@NodeToBrowse}",
                continuationPoint.NodeToBrowse);
            return;
        }

        var contextCopy = _systemContext.Copy();
        var toTake = continuationPoint.MaxResultsToReturn - references.Count;

        var nodeBrowser = continuationPoint.Data as INodeBrowser ?? nodeToBrowse.Node.CreateBrowser(contextCopy,
            continuationPoint.View,
            continuationPoint.ReferenceTypeId,
            continuationPoint.IncludeSubtypes,
            continuationPoint.BrowseDirection,
            null,
            null,
            false);

        using var scope = logger.BeginScope("Browsing node");
        var browsedValues = browser.Browse(continuationPoint.ResultMask, nodeBrowser).Take((int)toTake).ToList();

        foreach (var description in browsedValues.Where(e => e.IsSuccess)) references.Add(description.Value);

        foreach (var failure in browsedValues.Where(e => e.IsFailed))
            logger.LogError("Browsing resulted in error(s): {Errors}", failure.Errors);
    }

    /// <inheritdoc />
    public override void Write(OperationContext context, IList<WriteValue> nodesToWrite, IList<ServiceResult> errors)
    {
        var systemContext = _systemContext.Copy(context);
        var nodes = nodesToWrite.Where(e => !e.Processed && entityHandleManager.GetState(e.NodeId).IsSuccess).ToList();

        using var scope = logger.BeginScope("Writing nodes {NodesToWrite}", nodes.Select(e => e.NodeId));
        entityModifier.Write(systemContext, nodes);
    }


    /// <inheritdoc />
    public override void TranslateBrowsePath(OperationContext context, object sourceHandle,
        RelativePathElement relativePath,
        IList<ExpandedNodeId> targetIds, IList<NodeId> unresolvedTargetIds)
    {
        using var scope = logger.BeginScope("Translating browse path {Path}", relativePath.ToString());
        base.TranslateBrowsePath(context, sourceHandle, relativePath, targetIds, unresolvedTargetIds);
    }

    /// <inheritdoc />
    public override void Read(OperationContext context, double maxAge, IList<ReadValueId> nodesToRead,
        IList<DataValue> values,
        IList<ServiceResult> errors)
    {
        var filtered = nodesToRead
            .Where(e => !e.Processed && managedEntity.EntityNameSpaceIndex == e.NodeId.NamespaceIndex).ToList();
        if (!filtered.Any()) return;


        using var scope = logger.BeginScope("Reading values {ValuesToRead}", filtered.Select(e => e.NodeId).Distinct());
        var readResponses = entityReader.Read(filtered).ToList();
        readResponses.LogErrors(logger);
        foreach (var readResult in readResponses.Where(e => e.IsSuccess).Select(e => e.Value))
        {
            var placement = nodesToRead.IndexOf(readResult.Request);
            values[placement] = readResult.Response;
            readResult.Request.Processed = true;
        }
    }

    /// <inheritdoc />
    public override void HistoryRead(OperationContext context, HistoryReadDetails details,
        TimestampsToReturn timestampsToReturn,
        bool releaseContinuationPoints, IList<HistoryReadValueId> nodesToRead, IList<HistoryReadResult> results,
        IList<ServiceResult> errors)
    {
        using var scope = logger.BeginScope("Reading history for nodes {ToRead} with details type: {Details}",
            nodesToRead.Select(e => e.NodeId), details.TypeId);
        logger.LogWarning("History reading is not currently supported!");
    }


    /// <inheritdoc />
    public override void HistoryUpdate(OperationContext context, Type detailsType,
        IList<HistoryUpdateDetails> nodesToUpdate,
        IList<HistoryUpdateResult> results, IList<ServiceResult> errors)
    {
        using var scope = logger.BeginScope("Updating history for nodes {ToUpdate} with details type: {Type}",
            nodesToUpdate.Select(e => e.NodeId), detailsType.FullName);
        logger.LogWarning("History updating is not currently supported!");
    }

    /// <inheritdoc />
    public override void Call(OperationContext context, IList<CallMethodRequest> methodsToCall,
        IList<CallMethodResult> results,
        IList<ServiceResult> errors)
    {
        using var scope =
            logger.BeginScope("Attempting to call method {@Methods}", methodsToCall.Select(e => e.MethodId));
        logger.LogWarning("Calling methods is currently not supported!");
    }

    /// <inheritdoc />
    public override ServiceResult SubscribeToEvents(OperationContext context, object sourceId, uint subscriptionId,
        IEventMonitoredItem monitoredItem, bool unsubscribe)
    {
        using var scope =
            logger.BeginScope("Subscribing to events for monitored items {@MonitoredItem}", monitoredItem);
        logger.LogWarning("Subscribtion events are not yet supported");
        return StatusCodes.BadNotSupported;
    }

    /// <inheritdoc />
    public override ServiceResult SubscribeToAllEvents(OperationContext context, uint subscriptionId,
        IEventMonitoredItem monitoredItem,
        bool unsubscribe)
    {
        logger.LogInformation("Subscribing to all events for monitored items {@MonitoredItem}", monitoredItem);
        logger.LogWarning("Subscribtion events are not fully yet supported");
        return base.SubscribeToAllEvents(context, subscriptionId, monitoredItem, unsubscribe);
    }

    /// <inheritdoc />
    public override ServiceResult ConditionRefresh(OperationContext context, IList<IEventMonitoredItem> monitoredItems)
    {
        using var scope = logger.BeginScope("Refreshing conditions for monitored items {@MonitoredItems}",
            monitoredItems.Select(e => e.Id));
        logger.LogWarning("Subscribtion events are not yet supported");
        return StatusCodes.BadNotSupported;
    }

    /// <inheritdoc />
    public override void CreateMonitoredItems(OperationContext context, uint subscriptionId, double publishingInterval,
        TimestampsToReturn timestampsToReturn, IList<MonitoredItemCreateRequest> itemsToCreate,
        IList<ServiceResult> errors,
        IList<MonitoringFilterResult> filterErrors, IList<IMonitoredItem> monitoredItems,
        ref long globalIdCounter)
    {
        logger.LogWarning("Monitoring items are not yet supported. This is a meaningless operation.");

        using var beginScope = logger.BeginScope("Creating monitored items  {@MonitoredItems}",
            monitoredItems.Select(e => e.Id));

        foreach (var item in monitoredItems)
        {
            logger.LogInformation("Creating monitored item {Item}", item);
            logger.LogInformation("Created monitored {Item}", item.Id);
        }
    }

    /// <inheritdoc />
    public override void ModifyMonitoredItems(OperationContext context, TimestampsToReturn timestampsToReturn,
        IList<IMonitoredItem> monitoredItems,
        IList<MonitoredItemModifyRequest> itemsToModify, IList<ServiceResult> errors,
        IList<MonitoringFilterResult> filterErrors)
    {
        using var scope = logger.BeginScope("Modifying monitored items {@MonitoredItems}",
            monitoredItems.Select(e => e.Id));
        foreach (var item in monitoredItems) logger.LogInformation("Modifying monitored item {Item}...", item.Id);
    }

    /// <inheritdoc />
    public override void DeleteMonitoredItems(OperationContext context, IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        using var scope =
            logger.BeginScope("Deleting monitored items {@MonitoredItems}", monitoredItems.Select(e => e.Id));

        foreach (var item in monitoredItems) logger.LogInformation("Deleting monitored item {Item}...", item.Id);
    }

    /// <inheritdoc />
    public override void TransferMonitoredItems(OperationContext context, bool sendInitialValues,
        IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        using var scope = logger.BeginScope("Transferring monitored items {Items}",
            monitoredItems.Select(e => e.Id).ToNewlineSeparatedString());
        base.TransferMonitoredItems(context, sendInitialValues, monitoredItems, processedItems, errors);
    }

    /// <inheritdoc />
    public override void SetMonitoringMode(OperationContext context, MonitoringMode monitoringMode,
        IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        using var scope = logger.BeginScope("Setting monitoring modes for {@MonitoredItems} to {@MonitoringMode}",
            monitoredItems.Select(e => e.Id), monitoringMode);

        foreach (var item in monitoredItems)
            logger.LogInformation("Monitoring for {Item} set from {From} to {To}", item.Id, item.MonitoringMode,
                item.MonitoringMode);
    }

    /// <inheritdoc />
    public override void SessionClosing(OperationContext context, NodeId sessionId, bool deleteSubscriptions)
    {
        logger.LogInformation("Session {@Session} closing", sessionId);
        if (!deleteSubscriptions) return;

        using var beginScope = logger.BeginScope("Deleting subscriptions held by session {@Session}", sessionId);
        logger.LogWarning("Deleting subscriptions held by a session is not supported yet!");
    }

    /// <inheritdoc />
    public override bool IsNodeInView(OperationContext context, NodeId viewId, object nodeHandle)
    {
        using var scope = logger.BeginScope("Accessing if node {NodeHandle} is in view '{@View}'", nodeHandle, viewId);
        return base.IsNodeInView(context, viewId, nodeHandle);
    }

    /// <inheritdoc />
    public override NodeMetadata GetPermissionMetadata(OperationContext context, object targetHandle,
        BrowseResultMask resultMask,
        Dictionary<NodeId, List<object>> uniqueNodesServiceAttributesCache, bool permissionsOnly)
    {
        using var scope = logger.BeginScope("Getting permission metadata for {@TargetHandle}", targetHandle);
        if (!entityHandleManager.IsHandleToAnyRelatedNode(targetHandle))
            logger.LogWarning("Handle {@Handle} is not related to any nodes held by this manager", targetHandle);
        return base.GetPermissionMetadata(context, targetHandle, resultMask, uniqueNodesServiceAttributesCache,
            permissionsOnly);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing) base.Dispose();
    }

    private Result AddReference(KeyValuePair<NodeId, IList<IReference>> kvp)
    {
        if (entityHandleManager.IsManagedEntityHandle(kvp.Key))
            return referenceManager.AddReferencesToEntity(kvp.Value);

        if (entityHandleManager.IsManagedFolderHandle(kvp.Key))
            return referenceManager.AddReferencesToEntity(kvp.Value);

        return Result.Fail($"This manager does not hold a node with NodeId {kvp.Key}");
    }
}