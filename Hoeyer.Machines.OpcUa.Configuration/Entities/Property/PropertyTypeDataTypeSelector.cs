using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;
using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Configuration.Entities.Property;

public class PropertyTypeDataTypeSelector<TEntity>
{
    private PropertyTypeDataTypeSelector() { }
    private readonly PropertyConfiguration _propertyConfiguration;

    internal PropertyTypeDataTypeSelector(PropertyConfiguration propertyConfiguration)
    {
        _propertyConfiguration = propertyConfiguration;
    }
    public void AndIsOfType(BuiltInType builtInType)
    {   
        VerifyOrThrow(builtInType); 
        _propertyConfiguration.OpcUaNodeType = builtInType;
    }

    private void VerifyOrThrow(BuiltInType builtInType)
    {
        
       //TODO verify that the built int type is compatible with the property.
    }
}