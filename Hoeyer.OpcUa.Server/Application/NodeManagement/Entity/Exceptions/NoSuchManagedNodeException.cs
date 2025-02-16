using System;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.NodeManagement.Entity.Exceptions;

public class NoSuchManagedNodeException : InvalidOperationException
{
    /// <summary>
    /// Used when trying to access a node that is not held by an <see cref="EntityNodeManager"/>. 
    /// </summary>
    /// <param name="entityNode">The entity node held by the manager</param>
    /// <param name="nodeId">The NodeId of the sought node</param>
    public NoSuchManagedNodeException(EntityNode entityNode, NodeId nodeId):base($"The entity manager only holds the node {entityNode} and should never contain a node with node id {nodeId}")
    {
    }
}