using System.Reflection;

namespace Hoeyer.Machines.OpcUa.Client.Infrastructure.Configuration.Entities.Property;

public class PropertyConfigurator<TEntity>
{
    private readonly EntityConfiguration<TEntity> _context;
    private readonly PropertyInfo _propertyInfo;

    internal PropertyConfigurator(EntityConfiguration<TEntity> context, PropertyInfo propertyInfo)
    {
        _context = context;
        _propertyInfo = propertyInfo;
    }
    
    public PropertyTypeDataTypeSelector<TEntity> HasNodeId(NodeIdConfiguration nodeId)
    {
        var propertyConfiguration = _context.AddPropertyConfiguration(_propertyInfo, nodeId);
        return new PropertyTypeDataTypeSelector<TEntity>(propertyConfiguration);
    }

    /// <summary>
    /// Marks that the property should not be mapped.  
    /// </summary>
    public void IsNotMapped()
    {
        //noop
    }
}