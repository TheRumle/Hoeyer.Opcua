using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Application.RequestResponse;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Management;


internal sealed class EntityNodeManager(
    ManagedEntityNode managedEntity,
    IServerInternal server,
    IEntityHandleManager entityHandleManager,
    IEntityWriter entityWriter,
    IEntityBrowser browser,
    IEntityReader entityReader,
    IReferenceLinker referenceLinker,
    ILogger logger
) : CustomNodeManager(server, managedEntity.Namespace), IEntityNodeManager
{
    private readonly BaseObjectState _entity = managedEntity.BaseObject;
    private readonly StatusCodeResponseProcessorFactory _processorFactory = new(LogLevel.Error, LogLevel.Information);
    private readonly ServerSystemContext _systemContext = server.DefaultSystemContext;


    public IEntityNode ManagedEntity { get; } = managedEntity;

    /// <inheritdoc />
    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithScope("Creating address space and initializing {EntityBrowseName} property nodes", _entity.BrowseName)
            .WhenExecuting(() =>
            {
                referenceLinker.InitializeToExternals(externalReferences);
            });
    }


    /// <inheritdoc />
    public override void DeleteAddressSpace()
    {
        using var scope = logger.BeginScope("Disposing of entity {@Entity}", ManagedEntity);
        entityHandleManager.Dispose();
    }

    /// <inheritdoc />
    public override object? GetManagerHandle(NodeId nodeId)
    {
        return entityHandleManager.GetHandle(nodeId).ValueOrDefault;
    }

    /// <inheritdoc />
    public override void AddReferences(IDictionary<NodeId, IList<IReference>> references)
    {
        logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithScope("Adding references {References}", references)
            .WhenExecuting(() =>
            {
                foreach (var kvp in references)
                {
                    var result = referenceLinker.AddReferencesToEntity(kvp.Key, kvp.Value);
                    if (!result.IsSuccess)
                    {
                        logger.LogWarning(
                            "Failed to references from '{Node}' --> '{Targets}: {Error}'",
                            kvp.Key,
                            kvp.Value.Select(e => e.TargetId),
                            result.Errors.ToNewlineSeparatedString());
                    }
                    else
                    {
                        logger.LogInformation("Node {NodeId} now references targets: {References}", _entity.BrowseName,
                            kvp.Value.Select(e => e.TargetId));
                    }
                }
            });
    }

    /// <inheritdoc />
    public override ServiceResult DeleteReference(
        object sourceHandle,
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId,
        bool deleteBidirectional)
    {
        return logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithScope("Deleting references {Reference}", referenceTypeId)
            .WhenExecuting(() =>
            {
                if (!_entity.ReferenceExists(referenceTypeId, isInverse, targetId))
                {
                    logger.LogError("The {@Entity} does not reference node with reference type id '{@ReferenceTypeid}'",
                        _entity.BrowseName, referenceTypeId);
                    return StatusCodes.BadNodeIdUnknown;
                }

                var deletion = referenceLinker.RemoveReference(referenceTypeId, isInverse, targetId);
                if (deletion.IsSuccess)
                {
                    return ServiceResult.Good;
                }

                logger.LogError("Failed deleting reference {Reference}: {Errors}",
                    referenceTypeId,
                    deletion.Errors.ToNewlineSeparatedString());

                return StatusCodes.BadInvalidArgument;
            });
    }

    /// <inheritdoc />
    public override NodeMetadata GetNodeMetadata(OperationContext context, object targetHandle,
        BrowseResultMask resultMask)
    {
        return logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Getting metadata")
            .WithErrorMessage("Failed to get metadata for {@TargetHandle}", targetHandle)
            .WhenExecuting(() =>
            {
                if (!entityHandleManager.IsHandleToAnyRelatedNode(targetHandle))
                {
                    logger.LogError("The handle '{TargetHandle}' is not a handle related to entity {Entity}",
                        targetHandle,
                        _entity.BrowseName);
                    return null!;
                }

                var serverContext = _systemContext.Copy();
                return ManagedEntity.ConstructMetadata(serverContext);
            });
    }

    /// <inheritdoc />
    public override void Browse(OperationContext context, ref ContinuationPoint continuationPoint,
        IList<ReferenceDescription> references)
    {
        if (continuationPoint.NodeToBrowse is not IEntityNodeHandle nodeToBrowse)
        {
            return;
        }

        var cPoint = continuationPoint;
        continuationPoint = logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Browsing node")
            .WithErrorMessage("Failed to browse node")
            .WhenExecuting(() =>
            {
                return browser
                    .Browse(cPoint, nodeToBrowse)
                    .Then(result => references.AddRange(result.RelatedEntities))
                    .Then(result => logger.LogInformation("Browsed [{Result}]", result))
                    .ValueOrDefault
                    ?.ContinuationPoint;
            })!;
    }


    /// <inheritdoc />
    public override void TranslateBrowsePath(OperationContext context, object sourceHandle,
        RelativePathElement relativePath,
        IList<ExpandedNodeId> targetIds, IList<NodeId> unresolvedTargetIds)
    {
        logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Translating browse path " + relativePath)
            .WhenExecuting(() => base.TranslateBrowsePath(context, sourceHandle, relativePath, targetIds, unresolvedTargetIds));
    }

    /// <inheritdoc />
    public override void Read(OperationContext context,
        double maxAge,
        IList<ReadValueId> nodesToRead,
        IList<DataValue> values,
        IList<ServiceResult> errors)
    {
        var filtered = nodesToRead.Where(e => !e.Processed && entityHandleManager.IsManaged(e.NodeId)).ToList();
        if (!filtered.Any())
        {
            return;
        }
        logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Reading values " + ManagedEntity.BaseObject.BrowseName.Name)
            .WithErrorMessage("An unexpected error occurred when trying to read nodes")
            .WhenExecuting(() =>
            {
                _processorFactory.GetProcessorWithLoggingForFailedOnly("Read",
                    entityReader.ReadAttributes(filtered),
                    successful =>
                    {
                        successful.Request.Processed = true;
                        values[nodesToRead.IndexOf(successful.Request)] = successful.Response.DataValue;
                    },
                    errorResponse =>
                    {
                        errors[nodesToRead.IndexOf(errorResponse.Request)] = errorResponse.ResponseCode;
                    },
                    logger
                ).Process(e =>
                    (StatusCode.IsGood(e.ResponseCode)) || e.ResponseCode.Equals(StatusCodes.BadAttributeIdInvalid));
            });
    }

    /// <inheritdoc />
    public override void Write(OperationContext context, IList<WriteValue> nodesToWrite, IList<ServiceResult> errors)
    {
        var filtered = nodesToWrite.Where(e => !e.Processed && entityHandleManager.IsManaged(e.NodeId)).ToList();
        if (!filtered.Any())
        {
            return;
        }
        logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Writing values to entity")
            .WhenExecuting(() =>
            {
                var requestResponses = entityWriter.Write(filtered);
               _processorFactory.GetProcessorWithLoggingFor("Write",
                        requestResponses,
                        e => e.Request.Processed = true,
                        errorResponse => errors[nodesToWrite.IndexOf(errorResponse.Request)] = errorResponse.ResponseCode,
                        logger)
                    .Process(e => e.IsSuccess && StatusCode.IsGood(e.ResponseCode));
            });
    }

    /// <inheritdoc />
    public override void HistoryRead(OperationContext context, HistoryReadDetails details,
        TimestampsToReturn timestampsToReturn,
        bool releaseContinuationPoints, IList<HistoryReadValueId> nodesToRead, IList<HistoryReadResult> results,
        IList<ServiceResult> errors)
    {
        logger.LogWarning("History reading is not currently supported!");
        for (var index = 0; index < nodesToRead.Count; index++)
            errors[index] = StatusCodes.BadHistoryOperationUnsupported;
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
        using var scope = logger.BeginScope("Subscribing to events for monitored items {@MonitoredItem}", monitoredItem);
        if (!entityHandleManager.IsManagedEntityHandle(sourceId))
        {
            logger.LogWarning("Only subscriptions to entities (objects) are supported!");
            return ServiceResult.Create(StatusCodes.BadNotSupported,
                "Only subscriptions to entities (objects/views) are supported!");
        }
        
        
        //sourceId is handle
        // id is unique subscription
        logger.LogWarning("Subscribtion events are not yet supported");
        return StatusCodes.BadNotSupported;
    }

    /// <inheritdoc />
    public override ServiceResult SubscribeToAllEvents(OperationContext context, uint subscriptionId,
        IEventMonitoredItem monitoredItem,
        bool unsubscribe)
    {
        logger.LogInformation("Subscribing to all events for monitored item {@MonitoredItem}", monitoredItem);
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
        //https://chatgpt.com/share/67ff9f52-7a40-8001-8772-5400067a7ef5
        logger.LogWarning("Monitoring items are not yet supported. This is a meaningless operation.");

        using var beginScope = logger.BeginScope("Creating monitored items  {@MonitoredItems}",
            monitoredItems.Select(e => e.Id).ToArray());

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
        if (!deleteSubscriptions)
        {
            return;
        }

        using var beginScope = logger.BeginScope("Deleting subscriptions held by session {@Session}", sessionId);
        logger.LogWarning("Deleting subscriptions held by a session is not supported yet!");
    }

    /// <inheritdoc />
    /// This is not supported for EntityNodeManagers.
    public override bool IsNodeInView(OperationContext context, NodeId viewId, object nodeHandle)
    {
        return false;
    }

    /// <inheritdoc />
    public override NodeMetadata GetPermissionMetadata(OperationContext context, object targetHandle,
        BrowseResultMask resultMask,
        Dictionary<NodeId, List<object>> uniqueNodesServiceAttributesCache, bool permissionsOnly)
    {
        return logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Getting permission metadata...")
            .WhenExecuting(() => base.GetPermissionMetadata(context, targetHandle, resultMask,
                uniqueNodesServiceAttributesCache,
                permissionsOnly));
    }


    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            base.Dispose();
        }
    }
}