using Hoeyer.Machines.OpcUa.Client.Infrastructure.Configuration.Entities.Property;

namespace Hoeyer.Machines.OpcUa.Client.Infrastructure.Configuration.Entities.Builder;

internal class EntityFactorySelector<TEntity>(EntityConfiguration<TEntity> context) 
    : IEntityFactorySelector<TEntity> where TEntity : new()
{
    public  PropertySelector<TEntity>  WithEmptyConstructor<T>()
    {
        
        context.InstanceFactoryType = typeof(T);
        return new PropertySelector<TEntity>(context);
    } 
}