using Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities.Property;

namespace Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities.Builder;

public interface IEntityFactorySelector<TEntity>
{
    public PropertySelector<TEntity> WithEmptyConstructor<T>();
}