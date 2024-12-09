using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Context;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity.Property;

public class PropertyConfigurator
{
    private readonly OpcUaEntityConfiguration _context;
    private readonly PropertyInfo _propertyInfo;

    internal PropertyConfigurator(OpcUaEntityConfiguration context, PropertyInfo propertyInfo)
    {
        _context = context;
        _propertyInfo = propertyInfo;
    }
    
    public PropertyTypeDataTypeSelector HasBrowsableName(NodeIdConfiguration nodeId)
    {
        var propertyConfiguration = _context.AddPropertyConfiguration(_propertyInfo, nodeId);
        return new PropertyTypeDataTypeSelector(_context, propertyConfiguration);
    }

    /// <summary>
    /// Marks that the property should not be mapped.  
    /// </summary>
    public void IsNotMapped()
    {
        //noop
    }
}