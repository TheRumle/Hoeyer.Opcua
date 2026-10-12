using System.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

internal sealed record OpcPropertyTypeInfo(
    PropertyInfo PropertyInfo,
    PropertyState OpcProperty)
    : IOpcTypeInfo
{
    public readonly PropertyState OpcProperty = OpcProperty;
    public readonly PropertyInfo PropertyInfo = PropertyInfo;

    public BaseInstanceState InstanceState => OpcProperty;
}