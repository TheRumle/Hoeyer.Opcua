using Hoeyer.Machines.OpcUa.Configuration.Entities.Property;

namespace Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration.Builder;

public interface IOpcUaEntityConfigurationBuilder<T>
{
    IEntityFactorySelector<T> HasRootNodeIdentity(RootIdentity node);

}