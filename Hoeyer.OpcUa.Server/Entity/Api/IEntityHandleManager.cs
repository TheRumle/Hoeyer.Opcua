using System.Collections.Generic;
using FluentResults;
using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api;

public interface IEntityHandleManager
{
    public IEntityNodeHandle EntityHandle { get; }
    public IEnumerable<IEntityNodeHandle> PropertyHandles { get; }

    public bool IsHandleToAnyRelatedNode(object? handle);

    public bool IsManagedPropertyHandle(object? handle, out IEntityNodeHandle managedPropertyHandle);
    public bool IsManagedPropertyHandle(NodeId id, out IEntityNodeHandle managedPropertyHandle);
    public bool IsManagedEntityHandle(object? handle);
    public bool TryGetEntityHandle(NodeId id, out IEntityNodeHandle entityHandle);

    /// <summary>
    ///     Gets the handle object of property, entity, or folder if it exists
    /// </summary>
    /// <returns>
    ///     A result with the <see cref="BaseInstanceState" /> of the entity itself, the folder containing the entity or a
    ///     property of the entity.
    /// </returns>
    public Result<IEntityNodeHandle> GetHandle(NodeId nodeId);

    public Result<BaseInstanceState> GetState(NodeId nodeId);

    public bool IsManaged(NodeId nodeId);
}