using System.Collections.Generic;
using FluentResults;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IReferenceLinker
{
    Result IntitializeNodeWithReferences(IDictionary<NodeId, IList<IReference>> externalReferences);
    Result AddReferencesToEntity(IEnumerable<IReference> references);

    Result RemoveReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId);
}