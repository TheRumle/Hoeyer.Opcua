namespace Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities.Builder;

public interface IOpcUaEntityConfigurationBuilder<T>
{
    IEntityFactorySelector<T> HasRootNodeIdentity(RootIdentity node);

}