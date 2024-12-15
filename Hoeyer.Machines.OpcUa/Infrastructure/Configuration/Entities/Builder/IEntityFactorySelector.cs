using Hoeyer.Machines.OpcUa.Entities.Property;

namespace Hoeyer.Machines.OpcUa.Entities.Configuration.Builder;

public interface IEntityFactorySelector<TEntity>
{
    public PropertySelector<TEntity> WithEmptyConstructor<T>();
}