using System.Collections.Generic;
using FluentResults;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

public interface IEntityReferenceManager
{
    Result IntitializeNodeWithReferences(IDictionary<NodeId, IList<IReference>> externalReferences);
    Result AddReferencesToEntity(IEnumerable<IReference> references);
    Result AddReferencesToFolder(IEnumerable<IReference> references);

    Result DeleteReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId);
}