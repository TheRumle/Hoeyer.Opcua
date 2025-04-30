using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IManagedEntityNode : IEntityNode
{
    string Namespace { get; }
    ushort EntityNameSpaceIndex { get; }
}