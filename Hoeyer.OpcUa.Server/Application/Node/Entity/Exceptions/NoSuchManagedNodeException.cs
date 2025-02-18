using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;

public class NoSuchManagedNodeException : EntityNodeManagementException
{
    /// <summary>
    /// Used when trying to access a node that is not held by an <see cref="EntityNodeManager"/>. 
    /// </summary>
    /// <param name="entityNode">The entity node held by the manager</param>
    /// <param name="nodeId">The NodeId of the sought node</param>
    public NoSuchManagedNodeException(IEntityNode entityNode, NodeId nodeId):base($"The entity manager only holds the node {entityNode} and should never contain a node with node id {nodeId}")
    {
    }
    
    /// <summary>
    /// Used when trying to access a node that is not held by an <see cref="EntityNodeManager"/>. 
    /// </summary>
    /// <param name="entityNode">The entity node held by the manager</param>
    /// <param name="handle">The NodeId of the sought node</param>
    public NoSuchManagedNodeException(IEntityNode entityNode, object handle):base($"The entity manager only holds the node {entityNode} but was passed a reference handle to something else: {handle}")
    {
    }
}

public class InvalidHandleException(object handle) : EntityNodeManagementException($"Invalid source handle. The source handle '{handle}' is not a valid.");