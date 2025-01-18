using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Configuration.Entities.Property;

public class PropertyTypeDataTypeSelector<_>
{
    private readonly PropertyConfiguration _propertyConfiguration;

    internal PropertyTypeDataTypeSelector(PropertyConfiguration propertyConfiguration)
    {
        _propertyConfiguration = propertyConfiguration;
    }
    public void AndIsOfType(BuiltInType builtInType)
    {   
        _propertyConfiguration.OpcUaNodeType = builtInType;
    }
}