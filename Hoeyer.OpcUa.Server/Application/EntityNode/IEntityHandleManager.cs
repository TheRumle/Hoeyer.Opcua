using FluentResults;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IEntityHandleManager
{
    public bool IsHandleToAnyRelatedNode(object? handle);

    public bool IsManagedPropertyHandle(object? handle, out EntityNodeHandle<PropertyState> managedPropertyHandle);
    public bool IsManagedPropertyHandle(NodeId id, out EntityNodeHandle<PropertyState> managedPropertyHandle);
    public bool IsManagedFolderHandle(object? handle);
    public bool IsManagedFolderHandle(NodeId id, out EntityNodeHandle<FolderState> folderHandle);
    public bool IsManagedEntityHandle(object? handle);
    public bool IsManagedEntityHandle(NodeId id, out EntityNodeHandle<BaseObjectState> entityHandle);

    /// <summary>
    ///     Gets the handle object of property, entity, or folder if it exists
    /// </summary>
    /// <returns>
    ///     A result with the <see cref="BaseInstanceState" /> of the entity itself, the folder containing the entity or a
    ///     property of the entity.
    /// </returns>
    public Result<IEntityNodeHandle> GetHandle(NodeId nodeId);

    public Result<BaseInstanceState> GetState(NodeId nodeId);
}