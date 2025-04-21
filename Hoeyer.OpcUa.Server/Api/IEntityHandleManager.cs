using System;
using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityHandleManager : IDisposable
{
    public IEntityNodeHandle EntityHandle { get; }

    public bool IsHandleToAnyRelatedNode(object? handle);

    public bool IsManagedPropertyHandle(object? handle, out ManagedHandle<PropertyState> managedPropertyHandle);
    public bool IsManagedPropertyHandle(NodeId id, out ManagedHandle<PropertyState> managedPropertyHandle);
    public bool IsManagedEntityHandle(object? handle);
    public bool TryGetEntityHandle(NodeId id, out ManagedHandle<BaseObjectState> entityHandle);

    /// <summary>
    ///     Gets the handle object of property, entity, or folder if it exists
    /// </summary>
    /// <returns>
    ///     A result with the <see cref="BaseInstanceState" /> of the entity itself, the folder containing the entity or a
    ///     property of the entity.
    /// </returns>
    public IEntityNodeHandle? GetHandle(NodeId nodeId);

    public BaseInstanceState? GetState(NodeId nodeId);

    public bool IsManaged(NodeId nodeId);
}