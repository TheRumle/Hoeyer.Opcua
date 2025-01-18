namespace Hoeyer.OpcUa.Client.Configuration.Entities.Builder;

public interface IOpcUaEntityConfigurationBuilder<T>
{
    IEntityFactorySelector<T> HasRootNodeIdentity(RootIdentity node);
}