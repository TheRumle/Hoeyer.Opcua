namespace Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Builder;

public interface IOpcUaEntityConfigurationBuilder<T>
{
    IEntityFactorySelector<T> HasRootNodeIdentity(RootIdentity node);

}