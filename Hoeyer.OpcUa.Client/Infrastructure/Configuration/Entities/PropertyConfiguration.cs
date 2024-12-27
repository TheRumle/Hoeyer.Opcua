using System.Reflection;
using Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities.Builder;
using Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities.Property;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities;

public record PropertyConfiguration(PropertyInfo PropertyInfo, NodeIdConfiguration NodeIdConfiguration, RootIdentity Identity)
{
    private NodeId _nodeId => NodeIdConfiguration.ToNodeId(Identity);
    public NodeIdConfiguration NodeIdConfiguration { get; } = NodeIdConfiguration;
    public PropertyInfo PropertyInfo { get; } = PropertyInfo;
    public BuiltInType OpcUaNodeType { get; set; }
    public RootIdentity Identity { get; } = Identity;
    
    public NodeId GetNodeId() => _nodeId; 
}