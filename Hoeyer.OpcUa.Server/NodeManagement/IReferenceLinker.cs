using System.Collections.Generic;
using FluentResults;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.NodeManagement;

public interface IReferenceLinker
{
    Result InitializeToExternals(IDictionary<NodeId, IList<IReference>> externalReferences);
    /// <summary>
    /// Adds a reference entity --> element
    /// </summary>
    /// <returns></returns>
    Result AddReferencesToEntity(NodeId nodeId, IEnumerable<IReference> references);


    Result RemoveReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId);
}