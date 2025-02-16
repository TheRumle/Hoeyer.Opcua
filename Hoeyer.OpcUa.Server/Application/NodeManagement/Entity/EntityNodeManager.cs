using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Hoeyer.Common.Extensions.Exceptions;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.NodeManagement.Entity.Exceptions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.NodeManagement.Entity;

public sealed class EntityNodeManager : INodeManager2
{
    public IEnumerable<string> NamespaceUris => [EntityNamespace];
    public EntityNode EntityNode { get; set; }
    private FolderState EntityFolder => EntityNode.Folder;
    private BaseObjectState Entity => EntityNode.Entity;
    private readonly IServerInternal _server;
    private readonly ILogger _logger;
    private readonly ServerSystemContext _systemContext;
    private readonly IEntityNodeCreator _nodeCreator;
    private Dictionary<NodeId, BaseObjectState> _allNodes;
    public string EntityNamespace { get; }
    public ushort EntityNamespaceIndex { get; private set; }


    internal EntityNodeManager(
        string entityNamespace,
        IEntityNodeCreator nodeCreator,
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
        EntityNode = _nodeCreator.CreateEntityOpcUaNode(EntityFolder, EntityNamespaceIndex);
        EntityFolder.Create(_systemContext, EntityFolder.NodeId, EntityFolder.BrowseName, Entity.DisplayName, false);
        Entity.Create(_systemContext, Entity.NodeId, Entity.BrowseName, Entity.DisplayName, false);
        EntityFolder.AddChild(Entity);
        
        _allNodes = new Dictionary<NodeId, BaseObjectState>()
        {
            [Entity.NodeId] = Entity,
            [EntityFolder.NodeId] = EntityFolder
        };
    }

    /// <inheritdoc />
    public void DeleteAddressSpace()
    {
    }

    /// <inheritdoc />
    public object? GetManagerHandle(NodeId nodeId) => GetNodeHandle(nodeId);

    /// <summary>
    /// Tries to find a managed <see cref="BaseObjectState"/> that represents an Entity or null if no matching node exists.
    /// If the NodeId is invalid, then logs a warning specifying the node. 
    /// </summary>
    /// <param name="nodeId">The id of the BaseObjectState that represents a managed Entity </param>
    /// <returns></returns>
    private BaseObjectState? GetNodeHandle(NodeId nodeId)
    {
        if (NodeId.IsNull(nodeId) || nodeId.Identifier is null || !_allNodes.TryGetValue(nodeId, out var value))
        {
            _logger.LogWarning("Invalid NodeId. The NodeId is null or does not have an identifier. NodeId: {@NodeId}",
                nodeId);
            return null;
        }

        return value;
    }

    /// <summary>
    /// Returns the NodeID with 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    private BaseObjectState? GetNodeHandle(object? handle)
    {
        switch (handle)
        {
            case null:
                return GetNodeHandle(null!); //for logging purposes
            case NodeId id:
                return GetNodeHandle(id);
            case NodeState source:
                return GetNodeHandle(source.NodeId);
            default:
                _logger.LogError("Invalid source handle. The source handle {@SourceHandle} is not a valid NodeState",
                    handle);
                return null;
        }
    }

    /// <inheritdoc />
    public void AddReferences(IDictionary<NodeId, IList<IReference>> references)
    {
        var invalids = references
            .Where(e => !Entity.NodeId.Equals(e.Key))
            .Select(e => new NoSuchManagedNodeException(EntityNode, e.Key))
            .ToList();
        
        if (invalids.Any())
        {
            var exception = invalids.ToAggregateException();
            _logger.LogError(exception, "Tried to add a new reference to something that is not the managed Entity.");
            throw exception;
        }

        foreach (var r in references[Entity.NodeId])
        {
            Entity.AddReference(r.ReferenceTypeId, r.IsInverse, r.TargetId);
        }
    }

    /// <inheritdoc />
    public ServiceResult DeleteReference(
        object sourceHandle,
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId,
        bool deleteBidirectional)
    {
        BaseObjectState? source = GetNodeHandle(sourceHandle);
        if (source == null)
        {
            return StatusCodes.BadNodeIdUnknown;
        }

        source.RemoveReference(referenceTypeId, isInverse, targetId);
        if (deleteBidirectional && !targetId.IsAbsolute)
        {
            var target = GetNodeHandle((NodeId)targetId);
            target?.RemoveReference(referenceTypeId, !isInverse, source.NodeId);
        }

        _logger.LogInformation("Removed reference with ReferenceId {@ReferenceTypeId} from @{Source}", referenceTypeId,
            source);
        return ServiceResult.Good;
    }

    /// <inheritdoc />
    public NodeMetadata? GetNodeMetadata(OperationContext context, object targetHandle, BrowseResultMask resultMask)
    {
        var serverContext = _systemContext.Copy();
        var target = GetNodeHandle(targetHandle);
        if (target == null) return null;

        var values = target.ReadAttributes(
            serverContext,
            Attributes.WriteMask,
            Attributes.UserWriteMask,
            Attributes.DataType,
            Attributes.ValueRank,
            Attributes.ArrayDimensions,
            Attributes.AccessLevel,
            Attributes.UserAccessLevel,
            Attributes.EventNotifier,
            Attributes.Executable,
            Attributes.UserExecutable);


        // construct the metadata object.

        NodeMetadata metadata = new NodeMetadata(target, target.NodeId);

        metadata.NodeClass = target.NodeClass;
        metadata.BrowseName = target.BrowseName;
        metadata.DisplayName = target.DisplayName;

        if (values[0] != null && values[1] != null)
        {
            metadata.WriteMask = (AttributeWriteMask)((uint)values[0] & (uint)values[1]);
        }

        metadata.DataType = (NodeId)values[2];

        if (values[3] != null)
        {
            metadata.ValueRank = (int)values[3];
        }

        metadata.ArrayDimensions = (IList<uint>)values[4];

        if (values[5] != null && values[6] != null)
        {
            metadata.AccessLevel = (byte)(((byte)values[5]) & ((byte)values[6]));
        }

        if (values[7] != null)
        {
            metadata.EventNotifier = (byte)values[7];
        }

        if (values[8] != null && values[9] != null)
        {
            metadata.Executable = (bool)values[8] && (bool)values[9];
        }

        // get instance references.
        BaseInstanceState instance = target as BaseInstanceState;

        if (instance != null)
        {
            metadata.TypeDefinition = instance.TypeDefinitionId;
            metadata.ModellingRule = instance.ModellingRuleId;
        }

        // fill in the common attributes.
        return metadata;
    }

    /// <inheritdoc />
    public void Browse(OperationContext context, ref ContinuationPoint continuationPoint,
        IList<ReferenceDescription> references)
    {
        if (continuationPoint == null) throw new ArgumentNullException(nameof(continuationPoint));
        if (references == null) throw new ArgumentNullException(nameof(references));
        if (!ViewDescription.IsDefault(continuationPoint.View))
        {
            _logger.LogError("The continuation point is not a default view, and no other views are supported.");
            throw new InvalidBrowseException(
                "The continuation point is not a default view, and no other views are supported.");
        }

        BaseObjectState? source = GetNodeHandle(continuationPoint.NodeToBrowse);
        if (source == null)
        {
            _logger.LogError(
                "The entity manager cannot browse node with handle {@Handle} as the handle does not associate with a managed entity.",
                continuationPoint.NodeToBrowse);
            throw new InvalidBrowseException(continuationPoint.NodeToBrowse.ToString());
        }

        var systemContext = _systemContext.Copy(context);
        var browser = continuationPoint.Data as INodeBrowser
                      ?? source.CreateBrowser(systemContext,
                          continuationPoint.View,
                          continuationPoint.ReferenceTypeId,
                          continuationPoint.IncludeSubtypes,
                          continuationPoint.BrowseDirection,
                          null,
                          null,
                          false);


        for (IReference reference = browser.Next(); reference != null; reference = browser.Next())
        {
            // create the type definition reference.        
            ReferenceDescription description = GetReferenceDescription(reference, continuationPoint);

            if (description == null)
            {
                continue;
            }

            // check if limit reached.
            if (continuationPoint.MaxResultsToReturn != 0 && references.Count >= continuationPoint.MaxResultsToReturn)
            {
                browser.Push(reference);
                continuationPoint.Data = browser;
                return;
            }

            references.Add(description);
        }

        continuationPoint.Dispose();
        continuationPoint = null;
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
    public void Write(OperationContext context, IList<WriteValue> nodesToWrite, IList<ServiceResult> errors)
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

    private ReferenceDescription? GetReferenceDescription(IReference reference, ContinuationPoint continuationPoint)
    {
        ReferenceDescription description = new ReferenceDescription();
        description.NodeId = reference.TargetId;
        description.SetReferenceType(continuationPoint.ResultMask, reference.ReferenceTypeId, !reference.IsInverse);

        if (reference.TargetId.IsAbsolute)
        {
            return continuationPoint.NodeClassMask != 0 ? null : description;
        }

        var target = GetNodeHandle((NodeId)reference.TargetId);
        // the target may be a reference to a node in another node manager. In these cases
        // the target attributes must be fetched by the caller. The Unfiltered flag tells the
        // caller to do that.
        if (target == null)
        {
            description.Unfiltered = true;
            return description;
        }

        // apply node class filter.
        if (continuationPoint.NodeClassMask != 0 && ((continuationPoint.NodeClassMask & (uint)target.NodeClass) == 0))
        {
            return null;
        }

        description.SetTargetAttributes(continuationPoint.ResultMask,
            target.NodeClass,
            target.BrowseName,
            target.DisplayName,
            target.TypeDefinitionId);

        return description;
    }
}