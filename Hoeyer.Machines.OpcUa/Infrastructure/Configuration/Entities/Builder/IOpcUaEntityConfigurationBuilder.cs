namespace Hoeyer.Machines.OpcUa.Entities.Configuration.Builder;

public interface IOpcUaEntityConfigurationBuilder<T>
{
    IEntityFactorySelector<T> HasRootNodeIdentity(RootIdentity node);

}