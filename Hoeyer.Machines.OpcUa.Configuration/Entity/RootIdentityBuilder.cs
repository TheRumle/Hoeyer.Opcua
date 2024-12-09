using Hoeyer.Machines.OpcUa.Configuration.Entity.Context;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Property;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity;

internal class RootIdentityBuilder<TNodeType> : IOpcUaEntityConfigurationBuilder<TNodeType>, INodeConfigurationContextHolder
{
    public RootIdentityBuilder()
    {
        
    }
    public PropertySelector<TNodeType> HasRootNodeIdentity(RootIdentity node)
    {
        Context = new OpcUaEntityConfiguration(node);
        return new PropertySelector<TNodeType>(Context);
    }

    /// <inheritdoc />
    public OpcUaEntityConfiguration Context { get; set; } = null!;
}