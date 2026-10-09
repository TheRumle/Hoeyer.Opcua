using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

internal sealed record OpcPropertyTypeInfo : IOpcTypeInfo
{
    public readonly PropertyState OpcProperty;
    public readonly PropertyInfo PropertyInfo;
    public readonly NodeId? TypeId;

    public OpcPropertyTypeInfo(string browseName, PropertyInfo PropertyInfo, BaseInstanceState parent)
    {
        this.PropertyInfo = PropertyInfo;
        (TypeId, _) = ((NodeId? typeId, int rank))PropertyInfo.PropertyType.ToOpcTypeTuple();
        OpcProperty = GenericPropertyState
            .WithParent(parent)
            .WithName(browseName)
            .WithRootInformation(PropertyInfo)
            .Build();
    }

    public BaseInstanceState InstanceState => OpcProperty;
}

public interface IGenericPropertyStateBuilder
{
    public IGenericPropertyStateBuilder WithName(string value);
    public IGenericPropertyStateBuilder WithRootInformation(PropertyInfo value);
    public PropertyState Build();
}

public class GenericPropertyState(BaseInstanceState parent) : IGenericPropertyStateBuilder
{
    private string BrowseName { get; set; }
    private BaseInstanceState Parent => parent;
    private PropertyInfo PropertyInfo { get; set; }

    public static IGenericPropertyStateBuilder WithParent(BaseInstanceState parent) => new GenericPropertyState(parent);

    public IGenericPropertyStateBuilder WithName(string value)
    {
        BrowseName = value;
        return this;
    }

    public IGenericPropertyStateBuilder WithRootInformation(PropertyInfo value)
    {
        PropertyInfo = value;
        return this;
    }

    [SuppressMessage("Maintainability", "S3011", Justification = "This operation is safe")]
    public PropertyState Build()
    {
        var method = typeof(OpcTypeInfoUtils)
            .GetMethod(nameof(OpcTypeInfoUtils.CreateGenericProperty), BindingFlags.Public | BindingFlags.Static)!;

        var nodeId = new NodeId(parent.BrowseName.Name + "." + PropertyInfo.Name, parent.NodeId.NamespaceIndex);
        return (PropertyState)method.MakeGenericMethod(PropertyInfo.PropertyType)
            .Invoke(null, [BrowseName, nodeId, PropertyInfo, Parent]);
    }
}