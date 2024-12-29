using Hoeyer.OpcUa.Client.Configuration.Entities.Property;

namespace Hoeyer.OpcUa.Client.Configuration.Entities.Builder;

public interface IEntityFactorySelector<TEntity>
{
    public PropertySelector<TEntity> WithEmptyConstructor<T>();
}