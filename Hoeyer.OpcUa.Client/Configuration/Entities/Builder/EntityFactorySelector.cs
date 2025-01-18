using Hoeyer.OpcUa.Client.Configuration.Entities.Property;

namespace Hoeyer.OpcUa.Client.Configuration.Entities.Builder;

internal class EntityFactorySelector<TEntity>(EntityConfiguration<TEntity> context)
    : IEntityFactorySelector<TEntity> where TEntity : new()
{
    public PropertySelector<TEntity> WithEmptyConstructor<T>()
    {
        context.InstanceFactoryType = typeof(T);
        return new PropertySelector<TEntity>(context);
    }
}