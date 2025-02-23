using System.Collections.Generic;
using FluentResults;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IReferenceLinker
{
    Result IntitializeNodeWithReferences(IDictionary<NodeId, IList<IReference>> externalReferences);
    /// <summary>
    /// Adds a reference entity --> element
    /// </summary>
    /// <returns></returns>
    Result AddReferencesToEntity(NodeId kvpKey, IEnumerable<IReference> references);


    Result RemoveReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId);
}