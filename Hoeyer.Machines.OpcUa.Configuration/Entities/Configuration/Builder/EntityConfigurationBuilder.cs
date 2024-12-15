using Hoeyer.Machines.OpcUa.Configuration.Entities.Property;

namespace Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration.Builder;

internal class EntityConfigurationBuilder<TNodeType> : IOpcUaEntityConfigurationBuilder<TNodeType> where TNodeType : new()
{
    public EntityConfigurationBuilder()
    {
        
    }
    public IEntityFactorySelector<TNodeType> HasRootNodeIdentity(RootIdentity node)
    {
        Context = new EntityConfiguration<TNodeType>(node);
        return new EntityFactorySelector<TNodeType>(Context);
    }

    /// <inheritdoc />
    public EntityConfiguration<TNodeType> Context { get; set; } = null!;
    
}