namespace Hoeyer.OpcUa.Client.Configuration.Entities.Builder;

internal class EntityConfigurationBuilder<TNodeType> : IOpcUaEntityConfigurationBuilder<TNodeType>
    where TNodeType : new()
{
    /// <inheritdoc />
    public EntityConfiguration<TNodeType> EntityConfiguration { get; set; } = null!;

    public IEntityFactorySelector<TNodeType> HasRootNodeIdentity(RootIdentity node)
    {
        EntityConfiguration = new EntityConfiguration<TNodeType>(node);
        return new EntityFactorySelector<TNodeType>(EntityConfiguration);
    }
}