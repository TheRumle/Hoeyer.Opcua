using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions.Exceptions;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;
using Hoeyer.OpcUa.Server.Application.Node.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity;

public sealed class EntityNodeManager : INodeManager2
{
    public IEnumerable<string> NamespaceUris => [EntityNamespace];
    public EntityNode EntityNode { get; set; }
    private FolderState EntityFolder => EntityNode.Folder;
    private BaseObjectState Entity => EntityNode.Entity;
    private readonly IServerInternal _server;
    private readonly ILogger _logger;
    private readonly ServerSystemContext _systemContext;
    private readonly Func<ushort, EntityNode> _nodeCreator;
    private EntityHandleManager _handleManager;
    public string EntityNamespace { get; }
    public ushort EntityNamespaceIndex { get; private set; }
    private EntityModifier _entityModifier;
    private EntityBrowser _browser;


    internal EntityNodeManager(
        string entityNamespace,
        Func<ushort, EntityNode> nodeCreator,
        IServerInternal server,
        ILogger logger)
    {
        EntityNamespace = entityNamespace;
        EntityNode = null!;
        _nodeCreator = nodeCreator;
        _server = server;
        _systemContext = _server.DefaultSystemContext.Copy();
        _logger = logger;
    }


    /// <inheritdoc />
    public void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        EntityNamespaceIndex = _server.NamespaceUris.GetIndexOrAppend(EntityNamespace);
        EntityNode = _nodeCreator.Invoke(EntityNamespaceIndex);
        
        EntityFolder.Create(_systemContext, EntityFolder.NodeId, EntityFolder.BrowseName, Entity.DisplayName, false);
        Entity.Create(_systemContext, Entity.NodeId, Entity.BrowseName, Entity.DisplayName, false);
        EntityFolder.AddChild(Entity);


        _handleManager = new EntityHandleManager(EntityNode, _logger); 
        _entityModifier = new EntityModifier(EntityNode, _handleManager, _logger);
        _browser = new EntityBrowser(EntityNode, _handleManager, _logger);
    }


    /// <inheritdoc />
    public void DeleteAddressSpace()
    {
    }

    /// <inheritdoc />
    public object? GetManagerHandle(NodeId nodeId) => _handleManager.GetEntityHandle(nodeId);

    /// <inheritdoc />
    public void AddReferences(IDictionary<NodeId, IList<IReference>> references)
    {
        ThrowIfAnyExceptions(
            references.CreateEntityViolations(EntityNode, e => e.Key, e => new NoSuchManagedNodeException(EntityNode, e.Key))
            );
        _entityModifier.AddReferences(references);
    }
    
    private void ThrowIfAnyExceptions(IEnumerable<Exception> invalids)
    {
        var exceptions = invalids as Exception[] ?? invalids.ToArray();
        if (exceptions.Any())
        {
            var exception = exceptions.ToAggregateException();
            _logger.LogError(exception.Message);
            throw exception;
        }
    }

    /// <inheritdoc />
    public ServiceResult DeleteReference(
        object sourceHandle,
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId,
        bool deleteBidirectional) => _entityModifier.DeleteReference(sourceHandle, referenceTypeId, isInverse, targetId);

    /// <inheritdoc />
    public NodeMetadata GetNodeMetadata(OperationContext context, object targetHandle, BrowseResultMask resultMask)
    {
        if (!_handleManager.IsEntityHandle(targetHandle))
        {
            var e = new NoSuchManagedNodeException(EntityNode, targetHandle);
            _logger.LogError(e.Message);
            throw e;
        }

        var serverContext = _systemContext.Copy();
        return EntityNode.ConstructMetadata(serverContext, resultMask);
    }

    /// <inheritdoc />
    public void Browse(OperationContext context, ref ContinuationPoint continuationPoint,
        IList<ReferenceDescription> references)
    {
        var toTake = continuationPoint.MaxResultsToReturn - references.Count;
        if (toTake <= 0) return;
        
        foreach (var description  in _browser.Browse(continuationPoint, _systemContext.Copy(context)).Take((int)toTake))
        {
            references.Add(description);
        }
    }

    /// <inheritdoc />
    public void Write(OperationContext context, IList<WriteValue> nodesToWrite, IList<ServiceResult> errors)
    {
        var exceptions = nodesToWrite.CreateEntityViolations(EntityNode, wv => wv.NodeId, wv => new NoSuchManagedNodeException(EntityNode, wv.NodeId)).ToList();

        if (exceptions.Count != 0)
        {
            var e = exceptions.ToAggregateException();
            _logger.LogWarning(e, "Tried to write to nodes that are not related to EntityManager for entity {@Entity}", Entity.BrowseName);
            throw e;
        }
        var systemContext = _systemContext.Copy(context);
        _entityModifier.Write(systemContext, nodesToWrite);
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