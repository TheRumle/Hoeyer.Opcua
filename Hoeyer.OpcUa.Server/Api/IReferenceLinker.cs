using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api;

public interface IReferenceLinker
{
    void InitializeToExternals(IDictionary<NodeId, IList<IReference>> externalReferences);

    /// <summary>
    ///     Adds a reference entity --> element
    /// </summary>
    /// <returns></returns>
    void AddReferencesToEntity(NodeId nodeId, IEnumerable<IReference> references);


    void RemoveReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId);
}