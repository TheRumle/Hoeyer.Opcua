using Hoeyer.Machines.OpcUa.Configuration.Entity.Context;
using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity.Property;

public class PropertyTypeDataTypeSelector
{
    private PropertyTypeDataTypeSelector() { }
    private OpcUaEntityConfiguration _context;
    private readonly PropertyConfiguration _propertyConfiguration;

    internal PropertyTypeDataTypeSelector(OpcUaEntityConfiguration context, PropertyConfiguration propertyConfiguration)
    {
        _context = context;
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