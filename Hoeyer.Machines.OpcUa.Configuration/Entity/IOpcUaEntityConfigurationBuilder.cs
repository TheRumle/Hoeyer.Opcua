using Hoeyer.Machines.OpcUa.Configuration.Entity.Property;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity;

public interface IOpcUaEntityConfigurationBuilder<T>
{
    PropertySelector<T> HasRootNodeIdentity(RootIdentity node);

}