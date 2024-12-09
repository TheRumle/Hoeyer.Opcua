using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Property;
using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity.Context;

public record PropertyConfiguration(PropertyInfo PropertyInfo, NodeIdConfiguration NodeIdConfiguration, RootIdentity Identity)
{
    public NodeIdConfiguration NodeIdConfiguration { get; } = NodeIdConfiguration;
    public PropertyInfo PropertyInfo { get; } = PropertyInfo;
    public BuiltInType OpcUaNodeType { get; set; }
    public RootIdentity Identity { get; } = Identity;
    
    public NodeId GetNodeId => NodeIdConfiguration.ToNodeId(Identity); 
}