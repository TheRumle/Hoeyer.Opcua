using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core.Application.RequestResponse;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.Management;
using Hoeyer.OpcUa.Server.Api.Monitoring;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Hoeyer.OpcUa.Server.Application.Monitoring;
using Hoeyer.OpcUa.Server.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Management;


internal sealed class EntityNodeManager : CustomNodeManager, IEntityNodeManager
{
    private readonly BaseObjectState _entity;
    private readonly StatusCodeResponseProcessorFactory _processorFactory;
    private readonly ServerSystemContext _systemContext;
    private readonly IEntityWriter _entityWriter;
    private readonly IEntityBrowser _browser;
    private readonly IEntityReader _entityReader;
    private readonly IReferenceLinker _referenceLinker;
    private readonly IMonitoredItemManager _monitor;
    private readonly ILogger _logger;

    public EntityNodeManager(ManagedEntityNode managedEntity,
        IServerInternal server,
        IEntityChangedBroadcaster broadcaster,
        IEntityHandleManager entityHandleManager,
        IEntityBrowser browser,
        IEntityReader entityReader,
        IReferenceLinker referenceLinker,
        ILogger logger) : base(server, managedEntity.Namespace)
    {
        HandleManager = entityHandleManager;
        _entityReader = entityReader;
        _referenceLinker = referenceLinker;
        _logger = logger;
        _entity = managedEntity.BaseObject;
        _browser = browser;
        _entityWriter = new EntityStateChanger(managedEntity, broadcaster);
        _monitor = new MonitoredEntityItemsManager(broadcaster, HandleManager, new MonitoredItemFactory(() => server, () => this));
        _processorFactory = new StatusCodeResponseProcessorFactory(LogLevel.Error, LogLevel.Information, logger);
        _systemContext = server.DefaultSystemContext;
        ManagedEntity = managedEntity;
    }


    public IEntityNode ManagedEntity { get; }
    public IEntityHandleManager HandleManager { get; }

    /// <inheritdoc />
    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        _logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithScope("Creating address space and initializing {EntityBrowseName} property nodes", _entity.BrowseName)
            .WhenExecuting(() =>
            {
                _referenceLinker.InitializeToExternals(externalReferences);
            });
    }


    /// <inheritdoc />
    public override void DeleteAddressSpace()
    {
        using var scope = _logger.BeginScope("Disposing of entity {@Entity}", ManagedEntity);
        HandleManager.Dispose();
    }

    /// <inheritdoc />
    public override object? GetManagerHandle(NodeId nodeId)
    {
        return HandleManager.GetHandle(nodeId);
    }

    /// <inheritdoc />
    public override void AddReferences(IDictionary<NodeId, IList<IReference>> references)
    {
        using var scope = _logger.BeginScope("Adding references {References}", references);
        _logger.TryForEach(references,
            kvp => _referenceLinker.AddReferencesToEntity(kvp.Key, kvp.Value),
            
            onError: (kvp, exception) => _logger.LogWarning("Failed to references from '{Node}' --> '{Targets}: {Error}'",
                kvp.Key, kvp.Value.Select(e => e.TargetId), exception.Message),
            
            onEachSuccess: kvp => _logger.LogInformation("Node {NodeId} now references targets: {References}", _entity.BrowseName,
                kvp.Value.Select(e => e.TargetId))
            );
    }

    /// <inheritdoc />
    public override ServiceResult DeleteReference(
        object sourceHandle,
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId,
        bool deleteBidirectional)
    {
        using var scope = _logger.BeginScope("Deleting references {Reference}", referenceTypeId);
        if (!_entity.ReferenceExists(referenceTypeId, isInverse, targetId))
        {
            _logger.LogError("The {@Entity} does not reference node with reference type id '{@ReferenceTypeid}'",
                _entity.BrowseName, referenceTypeId);
            return StatusCodes.BadNodeIdUnknown;
        }

        var fail = _logger.Try(() => _referenceLinker.RemoveReference(referenceTypeId, isInverse, targetId));
        return fail == null ? ServiceResult.Good : StatusCodes.BadInvalidArgument;
    }

    /// <inheritdoc />
    public override NodeMetadata GetNodeMetadata(OperationContext context, object targetHandle,
        BrowseResultMask resultMask)
    {
        return _logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Getting metadata")
            .WithErrorMessage("Failed to get metadata for {@TargetHandle}", targetHandle)
            .WhenExecuting(() =>
            {
                if (!HandleManager.IsHandleToAnyRelatedNode(targetHandle))
                {
                    _logger.LogError("The handle '{TargetHandle}' is not a handle related to entity {Entity}",
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
        continuationPoint = _logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Browsing node")
            .WithErrorMessage("Failed to browse node")
            .WhenExecuting(() =>
            {
                return _browser
                    .Browse(cPoint, nodeToBrowse)
                    .Then(result => references.AddRange(result.RelatedEntities))
                    .Then(result => _logger.LogInformation("Browsed [{Result}]", result))
                    .ValueOrDefault
                    ?.ContinuationPoint;
            })!;
    }


    /// <inheritdoc />
    public override void TranslateBrowsePath(OperationContext context, object sourceHandle,
        RelativePathElement relativePath,
        IList<ExpandedNodeId> targetIds, IList<NodeId> unresolvedTargetIds)
    {
        _logger.LogCaughtExceptionAs(LogLevel.Error)
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
        var filtered = nodesToRead.Where(e => !e.Processed && HandleManager.IsManaged(e.NodeId)).ToList();
        if (!filtered.Any())
        {
            return;
        }
        _logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Reading values " + ManagedEntity.BaseObject.BrowseName.Name)
            .WithErrorMessage("An unexpected error occurred when trying to read nodes")
            .WhenExecuting(() =>
            {
                _processorFactory.GetProcessorWithLoggingForFailedOnly(_entityReader.ReadAttributes(filtered),
                    successful =>
                    {
                        successful.Request.Processed = true;
                        values[nodesToRead.IndexOf(successful.Request)] = successful.Response.DataValue;
                    },
                    errorResponse =>
                    {
                        errors[nodesToRead.IndexOf(errorResponse.Request)] = errorResponse.ResponseCode;
                    },
                    _logger
                ).Process(e =>
                    (StatusCode.IsGood(e.ResponseCode)) || e.ResponseCode.Equals(StatusCodes.BadAttributeIdInvalid));
            });
    }

    /// <inheritdoc />
    public override void Write(OperationContext context, IList<WriteValue> nodesToWrite, IList<ServiceResult> errors)
    {
        var filtered = nodesToWrite.Where(e => !e.Processed && HandleManager.IsManaged(e.NodeId)).ToList();
        if (!filtered.Any())
        {
            return;
        }
        _logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Writing values to entity")
            .WhenExecuting(() =>
            {
                var requestResponses = _entityWriter.Write(filtered);
               _processorFactory.GetProcessorWithLoggingFor(requestResponses,
                        e => e.Request.Processed = true,
                        errorResponse => errors[nodesToWrite.IndexOf(errorResponse.Request)] = errorResponse.ResponseCode)
                    .Process(e => e.IsSuccess && StatusCode.IsGood(e.ResponseCode));
            });
    }

    /// <inheritdoc />
    public override void HistoryRead(OperationContext context, HistoryReadDetails details,
        TimestampsToReturn timestampsToReturn,
        bool releaseContinuationPoints, IList<HistoryReadValueId> nodesToRead, IList<HistoryReadResult> results,
        IList<ServiceResult> errors)
    {
        _logger.LogWarning("History reading is not currently supported!");
        for (var index = 0; index < nodesToRead.Count; index++)
            errors[index] = StatusCodes.BadHistoryOperationUnsupported;
    }


    /// <inheritdoc />
    public override void HistoryUpdate(OperationContext context, Type detailsType,
        IList<HistoryUpdateDetails> nodesToUpdate,
        IList<HistoryUpdateResult> results, IList<ServiceResult> errors)
    {
        using var scope = _logger.BeginScope("Updating history for nodes {ToUpdate} with details type: {Type}",
            nodesToUpdate.Select(e => e.NodeId), detailsType.FullName);
        _logger.LogWarning("History updating is not currently supported!");
    }

    /// <inheritdoc />
    public override void Call(OperationContext context, IList<CallMethodRequest> methodsToCall,
        IList<CallMethodResult> results,
        IList<ServiceResult> errors)
    {
        using var scope =
            _logger.BeginScope("Attempting to call method {@Methods}", methodsToCall.Select(e => e.MethodId));
        _logger.LogWarning("Calling methods is currently not supported!");
    }

    /// <inheritdoc />
    public override ServiceResult SubscribeToEvents(OperationContext context, object sourceId, uint subscriptionId,
        IEventMonitoredItem monitoredItem, bool unsubscribe)
    {
        if (!HandleManager.IsHandleToAnyRelatedNode(sourceId))
        {
            _logger.LogWarning("Only subscriptions to entities and their properties are supported!");
            return ServiceResult.Create(StatusCodes.BadNotSupported,
                "Only subscriptions to entities (objects/views) are supported!");
        }
        
        _logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Subscribing to events")
            .WhenExecuting(() =>
            {
                Func<IEventMonitoredItem, uint, MonitorItemCreateResponse> action = unsubscribe ? _monitor.Unsubscribe : _monitor.Subscribe;
                var result = action.Invoke(monitoredItem, subscriptionId);
            });
        
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override ServiceResult SubscribeToAllEvents(OperationContext context, uint subscriptionId,
        IEventMonitoredItem monitoredItem,
        bool unsubscribe)
    {
        _logger.LogInformation("Subscribing to all events for monitored item {@MonitoredItem}", monitoredItem);
        _logger.LogWarning("Subscribtion events are not fully yet supported");
        return base.SubscribeToAllEvents(context, subscriptionId, monitoredItem, unsubscribe);
    }

    /// <inheritdoc />
    public override ServiceResult ConditionRefresh(OperationContext context, IList<IEventMonitoredItem> monitoredItems)
    {
        using var scope = _logger.BeginScope("Refreshing conditions for monitored items {@MonitoredItems}",
            monitoredItems.Select(e => e.Id));
        _logger.LogWarning("Subscribtion events are not yet supported");
        return StatusCodes.BadNotSupported;
    }

    /// <inheritdoc />
    public override void CreateMonitoredItems(OperationContext context, uint subscriptionId, double publishingInterval,
        TimestampsToReturn timestampsToReturn,
        IList<MonitoredItemCreateRequest> itemsToCreate,
        IList<ServiceResult> errors,
        IList<MonitoringFilterResult> filterErrors,
        IList<IMonitoredItem> monitoredItems,
        ref long globalIdCounter)
    {

        var toMonitor = itemsToCreate.Where(e => !e.Processed && HandleManager.IsManaged(e.ItemToMonitor.NodeId));
        _logger.LogCaughtExceptionAs(LogLevel.Error)
            .WithSessionContextScope(context, "Creating monitored items")
            .WhenExecuting(() =>
            {
                
                var res = _monitor.CreateMonitoredItem(context, toMonitor, subscriptionId, TimeSpan.FromMilliseconds(publishingInterval));
                var processor = _processorFactory.GetProcessorWithLoggingFor(res,
                        t => monitoredItems[t.OriginalIndex] = t.Response.DataValue,
                    e => errors[e.OriginalIndex] = e.ResponseCode);
                    
                processor.Process();


            });
 
    }

    /// <inheritdoc />
    public override void ModifyMonitoredItems(OperationContext context, TimestampsToReturn timestampsToReturn,
        IList<IMonitoredItem> monitoredItems,
        IList<MonitoredItemModifyRequest> itemsToModify,
        IList<ServiceResult> errors,
        IList<MonitoringFilterResult> filterErrors)
    {
        using var scope = _logger.BeginScope("Modifying monitored items {@MonitoredItems}",
            monitoredItems.Select(e => e.Id));
        foreach (var item in monitoredItems) _logger.LogInformation("Modifying monitored item {Item}...", item.Id);
    }

    /// <inheritdoc />
    public override void DeleteMonitoredItems(OperationContext context, IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        using var scope =
            _logger.BeginScope("Deleting monitored items {@MonitoredItems}", monitoredItems.Select(e => e.Id));

        foreach (var item in monitoredItems) _logger.LogInformation("Deleting monitored item {Item}...", item.Id);
    }

    /// <inheritdoc />
    public override void TransferMonitoredItems(OperationContext context, bool sendInitialValues,
        IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        using var scope = _logger.BeginScope("Transferring monitored items {Items}",
            monitoredItems.Select(e => e.Id).ToNewlineSeparatedString());
        base.TransferMonitoredItems(context, sendInitialValues, monitoredItems, processedItems, errors);
    }

    /// <inheritdoc />
    public override void SetMonitoringMode(OperationContext context, MonitoringMode monitoringMode,
        IList<IMonitoredItem> monitoredItems,
        IList<bool> processedItems, IList<ServiceResult> errors)
    {
        using var scope = _logger.BeginScope("Setting monitoring modes for {@MonitoredItems} to {@MonitoringMode}",
            monitoredItems.Select(e => e.Id), monitoringMode);

        foreach (var item in monitoredItems)
            _logger.LogInformation("Monitoring for {Item} set from {From} to {To}", item.Id, item.MonitoringMode,
                item.MonitoringMode);
    }

    /// <inheritdoc />
    public override void SessionClosing(OperationContext context, NodeId sessionId, bool deleteSubscriptions)
    {
        _logger.LogInformation("Session {@Session} closing", sessionId);
        if (!deleteSubscriptions)
        {
            return;
        }

        using var beginScope = _logger.BeginScope("Deleting subscriptions held by session {@Session}", sessionId);
        _logger.LogWarning("Deleting subscriptions held by a session is not supported yet!");
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
        return _logger.LogCaughtExceptionAs(LogLevel.Error)
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