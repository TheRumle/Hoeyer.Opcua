namespace Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Builder;

internal class EntityConfigurationBuilder<TNodeType> : IOpcUaEntityConfigurationBuilder<TNodeType> where TNodeType : new()
{
    public EntityConfigurationBuilder()
    {
        
    }
    public IEntityFactorySelector<TNodeType> HasRootNodeIdentity(RootIdentity node)
    {
        EntityConfiguration = new EntityConfiguration<TNodeType>(node);
        return new EntityFactorySelector<TNodeType>(EntityConfiguration);
    }

    /// <inheritdoc />
    public EntityConfiguration<TNodeType> EntityConfiguration { get; set; } = null!;
    
}