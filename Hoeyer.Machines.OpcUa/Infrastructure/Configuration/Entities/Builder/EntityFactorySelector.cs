using Hoeyer.Machines.OpcUa.Entities.Property;

namespace Hoeyer.Machines.OpcUa.Entities.Configuration.Builder;

internal class EntityFactorySelector<TEntity>(EntityConfiguration<TEntity> context) 
    : IEntityFactorySelector<TEntity> where TEntity : new()
{
    public  PropertySelector<TEntity>  WithEmptyConstructor<T>()
    {
        
        context.InstanceFactoryType = typeof(T);
        return new PropertySelector<TEntity>(context);
    } 
}