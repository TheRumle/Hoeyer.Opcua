using System.Reflection;
using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity.Exceptions;

public class IncompatibleTypesException(string s) : OpcuaConfigurationException(s)
{
    public IncompatibleTypesException(PropertyInfo info, BuiltInType type) : this($"{info.Name} is not compatible with {type}")
    {}
    
    public IncompatibleTypesException(PropertyInfo info) : this($"{info.Name} is of type {info.PropertyType.Name} and cannot be converted to a native Opc.Ua type.")
    {}

};