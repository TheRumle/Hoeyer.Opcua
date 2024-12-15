using Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Property;

namespace Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Builder;

public interface IEntityFactorySelector<TEntity>
{
    public PropertySelector<TEntity> WithEmptyConstructor<T>();
}