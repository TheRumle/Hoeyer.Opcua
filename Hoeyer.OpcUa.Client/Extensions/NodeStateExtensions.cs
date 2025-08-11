using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Extensions;

public static class NodeStateExtensions
{
    public static (NodeId Id, string Name) ToIdentityTuple(this NodeState nodeState) =>
        (nodeState.NodeId, nodeState.BrowseName.Name);
}